using System;

namespace SQLiteConsole_Local
{
    /// <summary>
    /// 通用分页信息
    /// </summary>
    public class PageInfo
    {
        private int _Page = 1;

        public int pageIndex
        {
            get
            {
                return _Page;
            }
            set
            {
                _Page = value;
            }
        }

        private int _PageSize = 20;

        public int pageSize
        {
            get
            {
                return _PageSize;
            }
            set
            {
                _PageSize = value;
            }
        }

        public int TotalCount { get; set; }

        private string _SortValue = null;

        public string sortField
        {
            get
            {
                return _SortValue;
            }
            set
            {
                _SortValue = value;
            }
        }

        //页数
        private int _pageCount;

        public Int32 PageCount
        {
            get
            {
                if (_pageCount == 0)
                {
                    _pageCount = TotalCount / pageSize;
                    if (TotalCount % pageSize > 0)
                    {
                        _pageCount++;
                    }
                }
                return _pageCount;
            }
            set
            {
                _pageCount = value;
            }
        }

        //当前页开始编号
        private int _rowStart;

        public Int32 RowStart
        {
            get
            {
                return _rowStart == 0 ? (pageIndex - 1) * pageSize + 1 : _rowStart;
            }
            set
            {
                _rowStart = value;
            }
        }

        //当前页结束编号
        private int _rowEnd;

        public Int32 RowEnd
        {
            get { return _rowEnd == 0 ? pageIndex * pageSize : _rowEnd; }
            set { _rowEnd = value; }
        }

        public string sortOrder { get; set; }
    }
}