﻿
// Copyright (C) 2016 Luca Piccioni
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
// USA

using System;
using System.Drawing;

using OSGeo.GDAL;

namespace OpenGL.Scene
{
	public partial class GeometryClipmapObject
	{
		/// <summary>
		/// Default terrain elevation factory.
		/// </summary>
		private class DefaultTerrainElevationFactory : ITerrainElevationFactory
		{
			#region Constructors

			/// <summary>
			/// Static constructor.
			/// </summary>
			static DefaultTerrainElevationFactory()
			{
				try {
					Gdal.AllRegister();
				} catch {
					
				}
			}

			public DefaultTerrainElevationFactory(string databaseRoot, double lat, double lon)
			{
				if (databaseRoot == null)
					throw new ArgumentNullException("databaseRoot");

				DatabaseRoot = databaseRoot;
				Latitude = lat;
				Longitude = lon;

				// Open the dataset.
				try {
					_DatabaseDataset = Gdal.OpenShared(databaseRoot, Access.GA_ReadOnly);

					if (_DatabaseDataset.RasterCount != 1)
						throw new NotSupportedException();

					using (Band band = _DatabaseDataset.GetRasterBand(1)) {
						switch (band.GetColorInterpretation()) {
							case ColorInterp.GCI_Undefined:
								switch (_DatabaseDataset.GetDriver().ShortName) {
									case "SRTMHGT":
									case "VRT":
										break;
									default:
										throw new NotSupportedException("unknown GDAL driver");
								}
								break;
							default:
								throw new NotSupportedException("unknown color interpretation");
						}
					}
				} catch {
					// Ensure GDAL object disposition
					if (_DatabaseDataset != null)
						_DatabaseDataset.Dispose();
					// Exception
					throw;
				}
			}

			#endregion

			#region Geographic Reference

			/// <summary>
			/// The root of the terrain elevation database.
			/// </summary>
			public readonly string DatabaseRoot;

			/// <summary>
			/// Dataset handle.
			/// </summary>
			private readonly Dataset _DatabaseDataset;

			/// <summary>
			/// Latitude coordinate, in degrees.
			/// </summary>
			public readonly double Latitude;

			/// <summary>
			/// Longitude coordinate, in degrees.
			/// </summary>
			public readonly double Longitude;

			#endregion

			#region ITerrainElevationFactory Implementation

			/// <summary>
			/// Creates the <see cref="ITerrainElevationSource"/> for the specified LOD.
			/// </summary>
			/// <param name="lod">
			/// A <see cref="UInt32"/> that specify the Level Of Detail of the terrain elevation data.
			/// </param>
			/// <param name="size">
			/// A <see cref="UInt32"/> that specify the size of the required terrain elevation source, in pixels, to be
			/// applied for both extents.
			/// </param>
			/// <param name="unitScale">
			/// A <see cref="Single"/> that specify the scale to be applied to a single terrain elevation fragment, in meters.
			/// </param>
			/// <returns>
			/// It returns a <see cref="ITerrainElevationSource"/> that satisfy the specified parameters.
			/// </returns>
			public ITerrainElevationSource CreateTerrainElevationSource(uint lod, uint size, float unitScale)
			{
				if (lod == 0)
					return (new DefaultTerrainElevationSource(_DatabaseDataset, Latitude, Longitude, size, (float)(Math.Pow(2.0, lod) * unitScale)));
				return (null);
			}

			#endregion
		}

		/// <summary>
		/// Default terrain elevation source.
		/// </summary>
		private class DefaultTerrainElevationSource : ITerrainElevationSource
		{
			#region Constructors

			public DefaultTerrainElevationSource(Dataset databaseDataset, double lat, double lon, uint size, float unitScale)
			{
				if (databaseDataset == null)
					throw new ArgumentNullException("databaseDataset");

				// IDisposable referenced
				_DatabaseDataset = databaseDataset;
				_DatabaseDataset.IncRef();

				// Determine current position
				double[] datasetTransform = new double[6], datasetInvTransform = new double[6];
				_DatabaseDataset.GetGeoTransform(datasetTransform);
				Gdal.InvGeoTransform(datasetTransform, datasetInvTransform);

				double xCurrentPosition, yCurrentPosition;
				Gdal.ApplyGeoTransform(datasetInvTransform, lon, lat, out xCurrentPosition, out yCurrentPosition);
				CurrentPosition = new Vertex2d(Math.Floor(xCurrentPosition), Math.Floor(yCurrentPosition));

				Latitude = lat;
				Longitude = lon;
				UnitScale = unitScale;

				_TerrainElevation = new Image(PixelLayout.GRAY16S, size, size);
				_TerrainElevation.IncRef();
			}

			#endregion

			#region Geographic Reference

			/// <summary>
			/// 
			/// </summary>
			private readonly Dataset _DatabaseDataset;

			/// <summary>
			/// Latitude coordinate, in degrees.
			/// </summary>
			public double Latitude;

			/// <summary>
			/// Longitude coordinate, in degrees.
			/// </summary>
			public double Longitude;

			/// <summary>
			/// 
			/// </summary>
			public readonly float UnitScale;

			/// <summary>
			/// The cartesian position corresponding to the coordinates <see cref="Latitude"/> and <see cref="Longitude"/>.
			/// </summary>
			public Vertex2d CurrentPosition;

			#endregion

			#region Terrain Elevation Management

			/// <summary>
			/// The updated terrain elevation.
			/// </summary>
			private readonly Image _TerrainElevation;

			#endregion

			#region ITerrainElevationSource Implementation

			/// <summary>
			/// Get the terrain elevation map corresponding to the specified position.
			/// </summary>
			/// <param name="viewPosition">
			/// The <see cref="Vertex3d"/> that specify the current view position, using an absolute cartesian reference system.
			/// </param>
			/// <returns>
			/// It returns the <see cref="Image"/> that contains the terrain elevation data corresponding to <paramref name="viewPosition"/>.
			/// </returns>
			public Image GetTerrainElevationMap(Vertex3d viewPosition)
			{
				Vertex2d view2dPosition = new Vertex2d(viewPosition.x, viewPosition.z);

				Vertex2d viewCacheDiff = view2dPosition - _LastCachePosition;
				if (Math.Abs(viewCacheDiff.x) < UnitScale && Math.Abs(viewCacheDiff.y) < UnitScale)
					return (null);

				// Apply clipmap offset
				Vertex2d viewOffset = view2dPosition / UnitScale;

				// Determine the dataset section to load
				Rectangle datasetSection = new Rectangle(0, 0, (int)_TerrainElevation.Width, (int)_TerrainElevation.Height);
				double x = viewOffset.x, y = viewOffset.y;

				double x1 = CurrentPosition.x + x - _TerrainElevation.Width / 2, x2 = CurrentPosition.x + x + _TerrainElevation.Width / 2;
				double y1 = CurrentPosition.y + y - _TerrainElevation.Height / 2, y2 = CurrentPosition.y + y + _TerrainElevation.Height / 2;

				datasetSection.X = (int)Math.Floor(x1);
				datasetSection.Y = (int)Math.Floor(y1);
				datasetSection.Width = (int)Math.Floor(x2 - x1);
				datasetSection.Height = (int)Math.Floor(y2 - y1);

				//System.Diagnostics.Trace.TraceInformation("{0}", datasetSection);

				// Update current terrain elevation, following the updated position
				ImageCodecCriteria datasetCriteria = new ImageCodecCriteria();
				datasetCriteria.ImageSection = datasetSection;
				GdalImageCodecPlugin.Load_GrayIndex(_DatabaseDataset, 1, datasetCriteria, _TerrainElevation);

				// Update current position
				//CurrentPosition = CurrentPosition + viewOffset;
				_LastCachePosition = view2dPosition;

				return (_TerrainElevation);
			}

			private Vertex2d _LastCachePosition;

			/// <summary>
			/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
			/// </summary>
			public void Dispose()
			{
				_TerrainElevation.DecRef();
				_DatabaseDataset.DecRef();
			}

			#endregion
		}
	}
}
