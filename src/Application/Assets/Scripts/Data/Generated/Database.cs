//#define MONO_STRICT
// 
//  ____  _     __  __      _        _ 
// |  _ \| |__ |  \/  | ___| |_ __ _| |
// | | | | '_ \| |\/| |/ _ \ __/ _` | |
// | |_| | |_) | |  | |  __/ || (_| | |
// |____/|_.__/|_|  |_|\___|\__\__,_|_|
//
// Auto-generated from main on 2015-02-24 01:09:17Z.
// Please visit http://code.google.com/p/dblinq2007/ for more information.
//
namespace VirtualHands.Data
{
	using System;
	using System.ComponentModel;
	using System.Data;
#if MONO_STRICT
	using System.Data.Linq;
#else   // MONO_STRICT
	using DbLinq.Data.Linq;
	using DbLinq.Vendor;
#endif  // MONO_STRICT
	using System.Data.Linq.Mapping;
	using System.Diagnostics;
	
	public partial class Main : DataContext
	{
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		#endregion
		
		
		public Main(string connectionString) : 
				base(connectionString)
		{
			this.OnCreated();
		}
		
		public Main(string connection, MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			this.OnCreated();
		}
		
		public Main(IDbConnection connection, MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			this.OnCreated();
		}
		
		public Table<ExifValue> ExifValues
		{
			get
			{
				return this.GetTable<ExifValue>();
			}
		}
		
		public Table<File> Files
		{
			get
			{
				return this.GetTable<File>();
			}
		}
		
		public Table<ImageStatistic> ImageStatistics
		{
			get
			{
				return this.GetTable<ImageStatistic>();
			}
		}
		
		public Table<MediaLibrary> MediaLibraries
		{
			get
			{
				return this.GetTable<MediaLibrary>();
			}
		}
	}
	
	#region Start MONO_STRICT
#if MONO_STRICT

	public partial class Main
	{
		
		public Main(IDbConnection connection) : 
				base(connection)
		{
			this.OnCreated();
		}
	}
	#region End MONO_STRICT
	#endregion
#else     // MONO_STRICT
	
	public partial class Main
	{
		
		public Main(IDbConnection connection) : 
				base(connection, new DbLinq.Sqlite.SqliteVendor())
		{
			this.OnCreated();
		}
		
		public Main(IDbConnection connection, IVendor sqlDialect) : 
				base(connection, sqlDialect)
		{
			this.OnCreated();
		}
		
		public Main(IDbConnection connection, MappingSource mappingSource, IVendor sqlDialect) : 
				base(connection, mappingSource, sqlDialect)
		{
			this.OnCreated();
		}
	}
	#region End Not MONO_STRICT
	#endregion
#endif     // MONO_STRICT
	#endregion
	
	[Table(Name="main.ExifValues")]
	public partial class ExifValue : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private int _fileID;
		
		private string _key;
		
		private string _value;
		
		private EntityRef<File> _file = new EntityRef<File>();
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnFileIDChanged();
		
		partial void OnFileIDChanging(int value);
		
		partial void OnKeyChanged();
		
		partial void OnKeyChanging(string value);
		
		partial void OnValueChanged();
		
		partial void OnValueChanging(string value);
		#endregion
		
		
		public ExifValue()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_fileID", Name="FileId", DbType="INTEGER", IsPrimaryKey=true, IsDbGenerated=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int FileID
		{
			get
			{
				return this._fileID;
			}
			set
			{
				if ((_fileID != value))
				{
					this.OnFileIDChanging(value);
					this.SendPropertyChanging();
					this._fileID = value;
					this.SendPropertyChanged("FileID");
					this.OnFileIDChanged();
				}
			}
		}
		
		[Column(Storage="_key", Name="Key", DbType="VARCHAR (50)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string Key
		{
			get
			{
				return this._key;
			}
			set
			{
				if (((_key == value) 
							== false))
				{
					this.OnKeyChanging(value);
					this.SendPropertyChanging();
					this._key = value;
					this.SendPropertyChanged("Key");
					this.OnKeyChanged();
				}
			}
		}
		
		[Column(Storage="_value", Name="Value", DbType="TEXT", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string Value
		{
			get
			{
				return this._value;
			}
			set
			{
				if (((_value == value) 
							== false))
				{
					this.OnValueChanging(value);
					this.SendPropertyChanging();
					this._value = value;
					this.SendPropertyChanged("Value");
					this.OnValueChanged();
				}
			}
		}
		
		#region Parents
		[Association(Storage="_file", OtherKey="ID", ThisKey="FileID", Name="fk_ExifValues_0", IsForeignKey=true)]
		[DebuggerNonUserCode()]
		public File File
		{
			get
			{
				return this._file.Entity;
			}
			set
			{
				if (((this._file.Entity == value) 
							== false))
				{
					if ((this._file.Entity != null))
					{
						File previousFile = this._file.Entity;
						this._file.Entity = null;
						previousFile.ExIfValues.Remove(this);
					}
					this._file.Entity = value;
					if ((value != null))
					{
						value.ExIfValues.Add(this);
						_fileID = value.ID;
					}
					else
					{
						_fileID = default(int);
					}
				}
			}
		}
		#endregion
		
		public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			System.ComponentModel.PropertyChangingEventHandler h = this.PropertyChanging;
			if ((h != null))
			{
				h(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(string propertyName)
		{
			System.ComponentModel.PropertyChangedEventHandler h = this.PropertyChanged;
			if ((h != null))
			{
				h(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[Table(Name="main.Files")]
	public partial class File : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private int _id;
		
		private System.DateTime _indexed;
		
		private string _path;
		
		private int _type;
		
		private EntitySet<ExifValue> _exIfValues;
		
		private EntitySet<ImageStatistic> _imageStatistics;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnIDChanged();
		
		partial void OnIDChanging(int value);
		
		partial void OnIndexedChanged();
		
		partial void OnIndexedChanging(System.DateTime value);
		
		partial void OnPathChanged();
		
		partial void OnPathChanging(string value);
		
		partial void OnTypeChanged();
		
		partial void OnTypeChanging(int value);
		#endregion
		
		
		public File()
		{
			_exIfValues = new EntitySet<ExifValue>(new Action<ExifValue>(this.ExIfValues_Attach), new Action<ExifValue>(this.ExIfValues_Detach));
			_imageStatistics = new EntitySet<ImageStatistic>(new Action<ImageStatistic>(this.ImageStatistics_Attach), new Action<ImageStatistic>(this.ImageStatistics_Detach));
			this.OnCreated();
		}
		
		[Column(Storage="_id", Name="Id", DbType="INTEGER", IsPrimaryKey=true, IsDbGenerated=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int ID
		{
			get
			{
				return this._id;
			}
			set
			{
				if ((_id != value))
				{
					this.OnIDChanging(value);
					this.SendPropertyChanging();
					this._id = value;
					this.SendPropertyChanged("ID");
					this.OnIDChanged();
				}
			}
		}
		
		[Column(Storage="_indexed", Name="Indexed", DbType="TIMESTAMP", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public System.DateTime Indexed
		{
			get
			{
				return this._indexed;
			}
			set
			{
				if ((_indexed != value))
				{
					this.OnIndexedChanging(value);
					this.SendPropertyChanging();
					this._indexed = value;
					this.SendPropertyChanged("Indexed");
					this.OnIndexedChanged();
				}
			}
		}
		
		[Column(Storage="_path", Name="Path", DbType="TEXT", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public string Path
		{
			get
			{
				return this._path;
			}
			set
			{
				if (((_path == value) 
							== false))
				{
					this.OnPathChanging(value);
					this.SendPropertyChanging();
					this._path = value;
					this.SendPropertyChanged("Path");
					this.OnPathChanged();
				}
			}
		}
		
		[Column(Storage="_type", Name="Type", DbType="INTEGER", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int Type
		{
			get
			{
				return this._type;
			}
			set
			{
				if ((_type != value))
				{
					this.OnTypeChanging(value);
					this.SendPropertyChanging();
					this._type = value;
					this.SendPropertyChanged("Type");
					this.OnTypeChanged();
				}
			}
		}
		
		#region Children
		[Association(Storage="_exIfValues", OtherKey="FileID", ThisKey="ID", Name="fk_ExifValues_0")]
		[DebuggerNonUserCode()]
		public EntitySet<ExifValue> ExIfValues
		{
			get
			{
				return this._exIfValues;
			}
			set
			{
				this._exIfValues = value;
			}
		}
		
		[Association(Storage="_imageStatistics", OtherKey="FileID", ThisKey="ID", Name="fk_ImageStatistics_0")]
		[DebuggerNonUserCode()]
		public EntitySet<ImageStatistic> ImageStatistics
		{
			get
			{
				return this._imageStatistics;
			}
			set
			{
				this._imageStatistics = value;
			}
		}
		#endregion
		
		public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			System.ComponentModel.PropertyChangingEventHandler h = this.PropertyChanging;
			if ((h != null))
			{
				h(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(string propertyName)
		{
			System.ComponentModel.PropertyChangedEventHandler h = this.PropertyChanged;
			if ((h != null))
			{
				h(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
		
		#region Attachment handlers
		private void ExIfValues_Attach(ExifValue entity)
		{
			this.SendPropertyChanging();
			entity.File = this;
		}
		
		private void ExIfValues_Detach(ExifValue entity)
		{
			this.SendPropertyChanging();
			entity.File = null;
		}
		
		private void ImageStatistics_Attach(ImageStatistic entity)
		{
			this.SendPropertyChanging();
			entity.File = this;
		}
		
		private void ImageStatistics_Detach(ImageStatistic entity)
		{
			this.SendPropertyChanging();
			entity.File = null;
		}
		#endregion
	}
	
	[Table(Name="main.ImageStatistics")]
	public partial class ImageStatistic : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private System.Nullable<double> _entropy;
		
		private int _fileID;
		
		private System.Nullable<bool> _hasExif;
		
		private System.Nullable<int> _height;
		
		private System.Nullable<double> _hue;
		
		private System.Nullable<double> _kurtosis;
		
		private System.Nullable<System.DateTime> _lastModified;
		
		private System.Nullable<double> _lightness;
		
		private System.Nullable<double> _mean;
		
		private System.Nullable<double> _saturation;
		
		private System.Nullable<double> _skewness;
		
		private System.Nullable<double> _variance;
		
		private System.Nullable<int> _version;
		
		private System.Nullable<int> _width;
		
		private EntityRef<File> _file = new EntityRef<File>();
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnEntropyChanged();
		
		partial void OnEntropyChanging(System.Nullable<double> value);
		
		partial void OnFileIDChanged();
		
		partial void OnFileIDChanging(int value);
		
		partial void OnHasExifChanged();
		
		partial void OnHasExifChanging(System.Nullable<bool> value);
		
		partial void OnHeightChanged();
		
		partial void OnHeightChanging(System.Nullable<int> value);
		
		partial void OnHueChanged();
		
		partial void OnHueChanging(System.Nullable<double> value);
		
		partial void OnKurtosisChanged();
		
		partial void OnKurtosisChanging(System.Nullable<double> value);
		
		partial void OnLastModifiedChanged();
		
		partial void OnLastModifiedChanging(System.Nullable<System.DateTime> value);
		
		partial void OnLightnessChanged();
		
		partial void OnLightnessChanging(System.Nullable<double> value);
		
		partial void OnMeanChanged();
		
		partial void OnMeanChanging(System.Nullable<double> value);
		
		partial void OnSaturationChanged();
		
		partial void OnSaturationChanging(System.Nullable<double> value);
		
		partial void OnSkewnessChanged();
		
		partial void OnSkewnessChanging(System.Nullable<double> value);
		
		partial void OnVarianceChanged();
		
		partial void OnVarianceChanging(System.Nullable<double> value);
		
		partial void OnVersionChanged();
		
		partial void OnVersionChanging(System.Nullable<int> value);
		
		partial void OnWidthChanged();
		
		partial void OnWidthChanging(System.Nullable<int> value);
		#endregion
		
		
		public ImageStatistic()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_entropy", Name="Entropy", DbType="DOUBLE", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<double> Entropy
		{
			get
			{
				return this._entropy;
			}
			set
			{
				if ((_entropy != value))
				{
					this.OnEntropyChanging(value);
					this.SendPropertyChanging();
					this._entropy = value;
					this.SendPropertyChanged("Entropy");
					this.OnEntropyChanged();
				}
			}
		}
		
		[Column(Storage="_fileID", Name="FileId", DbType="INTEGER", IsPrimaryKey=true, IsDbGenerated=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int FileID
		{
			get
			{
				return this._fileID;
			}
			set
			{
				if ((_fileID != value))
				{
					this.OnFileIDChanging(value);
					this.SendPropertyChanging();
					this._fileID = value;
					this.SendPropertyChanged("FileID");
					this.OnFileIDChanged();
				}
			}
		}
		
		[Column(Storage="_hasExif", Name="HasExif", DbType="BOOLEAN", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<bool> HasExif
		{
			get
			{
				return this._hasExif;
			}
			set
			{
				if ((_hasExif != value))
				{
					this.OnHasExifChanging(value);
					this.SendPropertyChanging();
					this._hasExif = value;
					this.SendPropertyChanged("HasExif");
					this.OnHasExifChanged();
				}
			}
		}
		
		[Column(Storage="_height", Name="Height", DbType="INTEGER", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<int> Height
		{
			get
			{
				return this._height;
			}
			set
			{
				if ((_height != value))
				{
					this.OnHeightChanging(value);
					this.SendPropertyChanging();
					this._height = value;
					this.SendPropertyChanged("Height");
					this.OnHeightChanged();
				}
			}
		}
		
		[Column(Storage="_hue", Name="Hue", DbType="DOUBLE", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<double> Hue
		{
			get
			{
				return this._hue;
			}
			set
			{
				if ((_hue != value))
				{
					this.OnHueChanging(value);
					this.SendPropertyChanging();
					this._hue = value;
					this.SendPropertyChanged("Hue");
					this.OnHueChanged();
				}
			}
		}
		
		[Column(Storage="_kurtosis", Name="Kurtosis", DbType="DOUBLE", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<double> Kurtosis
		{
			get
			{
				return this._kurtosis;
			}
			set
			{
				if ((_kurtosis != value))
				{
					this.OnKurtosisChanging(value);
					this.SendPropertyChanging();
					this._kurtosis = value;
					this.SendPropertyChanged("Kurtosis");
					this.OnKurtosisChanged();
				}
			}
		}
		
		[Column(Storage="_lastModified", Name="LastModified", DbType="DATETIME", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<System.DateTime> LastModified
		{
			get
			{
				return this._lastModified;
			}
			set
			{
				if ((_lastModified != value))
				{
					this.OnLastModifiedChanging(value);
					this.SendPropertyChanging();
					this._lastModified = value;
					this.SendPropertyChanged("LastModified");
					this.OnLastModifiedChanged();
				}
			}
		}
		
		[Column(Storage="_lightness", Name="Lightness", DbType="DOUBLE", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<double> Lightness
		{
			get
			{
				return this._lightness;
			}
			set
			{
				if ((_lightness != value))
				{
					this.OnLightnessChanging(value);
					this.SendPropertyChanging();
					this._lightness = value;
					this.SendPropertyChanged("Lightness");
					this.OnLightnessChanged();
				}
			}
		}
		
		[Column(Storage="_mean", Name="Mean", DbType="DOUBLE", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<double> Mean
		{
			get
			{
				return this._mean;
			}
			set
			{
				if ((_mean != value))
				{
					this.OnMeanChanging(value);
					this.SendPropertyChanging();
					this._mean = value;
					this.SendPropertyChanged("Mean");
					this.OnMeanChanged();
				}
			}
		}
		
		[Column(Storage="_saturation", Name="Saturation", DbType="DOUBLE", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<double> Saturation
		{
			get
			{
				return this._saturation;
			}
			set
			{
				if ((_saturation != value))
				{
					this.OnSaturationChanging(value);
					this.SendPropertyChanging();
					this._saturation = value;
					this.SendPropertyChanged("Saturation");
					this.OnSaturationChanged();
				}
			}
		}
		
		[Column(Storage="_skewness", Name="Skewness", DbType="DOUBLE", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<double> Skewness
		{
			get
			{
				return this._skewness;
			}
			set
			{
				if ((_skewness != value))
				{
					this.OnSkewnessChanging(value);
					this.SendPropertyChanging();
					this._skewness = value;
					this.SendPropertyChanged("Skewness");
					this.OnSkewnessChanged();
				}
			}
		}
		
		[Column(Storage="_variance", Name="Variance", DbType="DOUBLE", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<double> Variance
		{
			get
			{
				return this._variance;
			}
			set
			{
				if ((_variance != value))
				{
					this.OnVarianceChanging(value);
					this.SendPropertyChanging();
					this._variance = value;
					this.SendPropertyChanged("Variance");
					this.OnVarianceChanged();
				}
			}
		}
		
		[Column(Storage="_version", Name="Version", DbType="INTEGER", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<int> Version
		{
			get
			{
				return this._version;
			}
			set
			{
				if ((_version != value))
				{
					this.OnVersionChanging(value);
					this.SendPropertyChanging();
					this._version = value;
					this.SendPropertyChanged("Version");
					this.OnVersionChanged();
				}
			}
		}
		
		[Column(Storage="_width", Name="Width", DbType="INTEGER", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<int> Width
		{
			get
			{
				return this._width;
			}
			set
			{
				if ((_width != value))
				{
					this.OnWidthChanging(value);
					this.SendPropertyChanging();
					this._width = value;
					this.SendPropertyChanged("Width");
					this.OnWidthChanged();
				}
			}
		}
		
		#region Parents
		[Association(Storage="_file", OtherKey="ID", ThisKey="FileID", Name="fk_ImageStatistics_0", IsForeignKey=true)]
		[DebuggerNonUserCode()]
		public File File
		{
			get
			{
				return this._file.Entity;
			}
			set
			{
				if (((this._file.Entity == value) 
							== false))
				{
					if ((this._file.Entity != null))
					{
						File previousFile = this._file.Entity;
						this._file.Entity = null;
						previousFile.ImageStatistics.Remove(this);
					}
					this._file.Entity = value;
					if ((value != null))
					{
						value.ImageStatistics.Add(this);
						_fileID = value.ID;
					}
					else
					{
						_fileID = default(int);
					}
				}
			}
		}
		#endregion
		
		public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			System.ComponentModel.PropertyChangingEventHandler h = this.PropertyChanging;
			if ((h != null))
			{
				h(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(string propertyName)
		{
			System.ComponentModel.PropertyChangedEventHandler h = this.PropertyChanged;
			if ((h != null))
			{
				h(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[Table(Name="main.MediaLibrary")]
	public partial class MediaLibrary : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private int _id;
		
		private string _path;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnIDChanged();
		
		partial void OnIDChanging(int value);
		
		partial void OnPathChanged();
		
		partial void OnPathChanging(string value);
		#endregion
		
		
		public MediaLibrary()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_id", Name="Id", DbType="INTEGER", IsPrimaryKey=true, IsDbGenerated=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int ID
		{
			get
			{
				return this._id;
			}
			set
			{
				if ((_id != value))
				{
					this.OnIDChanging(value);
					this.SendPropertyChanging();
					this._id = value;
					this.SendPropertyChanged("ID");
					this.OnIDChanged();
				}
			}
		}
		
		[Column(Storage="_path", Name="Path", DbType="TEXT", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public string Path
		{
			get
			{
				return this._path;
			}
			set
			{
				if (((_path == value) 
							== false))
				{
					this.OnPathChanging(value);
					this.SendPropertyChanging();
					this._path = value;
					this.SendPropertyChanged("Path");
					this.OnPathChanged();
				}
			}
		}
		
		public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			System.ComponentModel.PropertyChangingEventHandler h = this.PropertyChanging;
			if ((h != null))
			{
				h(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(string propertyName)
		{
			System.ComponentModel.PropertyChangedEventHandler h = this.PropertyChanged;
			if ((h != null))
			{
				h(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
