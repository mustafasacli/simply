namespace Simply.Common.Objects
{
    /// <summary>
    /// The simple format setting.
    /// </summary>
    public class SimpleFormatSetting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleFormatSetting"/> class.
        /// </summary>
        public SimpleFormatSetting()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleFormatSetting"/> class.
        /// </summary>
        /// <param name="addTab">If true, add tab.</param>
        /// <param name="addNewLine">If true, add new line.</param>
        /// <param name="datetimeFormat">The datetime format(standart format: yyyy-MM-dd HH:mm:ss.fffffff).</param>
        public SimpleFormatSetting(bool addTab, bool addNewLine, string datetimeFormat)
        {
            this.AddTab = addTab;
            this.AddNewLine = addNewLine;
            this.DatetimeFormat = datetimeFormat;
        }

        /// <summary>
        /// Gets or sets a value indicating whether add tab.
        /// </summary>
        public bool AddTab
        { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether add new line.
        /// </summary>
        public bool AddNewLine
        { get; set; } = true;

        /// <summary>
        /// Gets or sets the datetime format.
        /// </summary>
        public string DatetimeFormat
        { get; set; } = "yyyy-MM-dd HH:mm:ss.fffffff";

        /// <summary>
        /// Gets or sets the main xml node name. it is used for xml formatting.
        /// </summary>
        public string MainXmlNodeName
        { get; set; } = "row";

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <returns>A SimpleFormatSetting.</returns>
        public static SimpleFormatSetting New()
        { return new SimpleFormatSetting(); }

        /// <summary>
        /// Sets the new line and returns current instance.
        /// </summary>
        /// <param name="addNewLine">If true, add new line.</param>
        /// <returns>A SimpleFormatSetting.</returns>
        public SimpleFormatSetting SetNewLine(bool addNewLine)
        {
            this.AddNewLine = addNewLine;
            return this;
        }

        /// <summary>
        /// Sets the tab and returns current instance.
        /// </summary>
        /// <param name="addTab">If true, add tab.</param>
        /// <returns>A SimpleFormatSetting.</returns>
        public SimpleFormatSetting SetTab(bool addTab)
        {
            this.AddTab = addTab;
            return this;
        }

        /// <summary>
        /// Sets the datetime format and returns current instance.
        /// </summary>
        /// <param name="datetimeFormat">The datetime format.</param>
        /// <returns>A SimpleFormatSetting.</returns>
        public SimpleFormatSetting SetDatetimeFormat(string datetimeFormat)
        {
            this.DatetimeFormat = datetimeFormat;
            return this;
        }

        /// <summary>
        /// Sets the main xml node name and returns current instance.
        /// </summary>
        /// <param name="mainXmlNodeName">The main xml node name.</param>
        /// <returns>A SimpleFormatSetting.</returns>
        public SimpleFormatSetting SetMainXmlNodeName(string mainXmlNodeName)
        {
            this.MainXmlNodeName = mainXmlNodeName;
            return this;
        }
    }
}