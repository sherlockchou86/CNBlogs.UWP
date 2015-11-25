using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CNBlogs.UWP.HTTP;
using CNBlogs.UWP.Models;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace CNBlogs.UWP.Data
{
    class CN10TopDiggList : ObservableCollection<CNBlog>, ISupportIncrementalLoading
    {
        private bool _busy = false;
        private bool _has_more_items = false;
        private int _current_page = 1;
        public event DataLoadingEventHandler DataLoading;
        public event DataLoadedEventHandler DataLoaded;

        public int TotalCount
        {
            get; set;
        }
        public bool HasMoreItems
        {
            get
            {
                if (_busy)
                    return false;
                else
                    return _has_more_items;
            }
            private set
            {
                _has_more_items = value;
            }
        }
        public CN10TopDiggList()
        {
            HasMoreItems = true;
        }
        public void DoRefresh()
        {
            _current_page = 1;
            TotalCount = 0;
            Clear();
            HasMoreItems = true;
        }
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return InnerLoadMoreItemsAsync(count).AsAsyncOperation();
        }
        private async Task<LoadMoreItemsResult> InnerLoadMoreItemsAsync(uint expectedCount)
        {
            _busy = true;
            var actualCount = 0;
            List<CNBlog> list = null;
            try
            {
                if (DataLoading != null)
                {
                    DataLoading();
                }
                list = await BlogService.Get10TopDiggsAysnc(_current_page * 20);
            }
            catch (Exception)
            {
                HasMoreItems = false;
            }

            if (list != null && list.Any() && list.Count > TotalCount)
            {
                int index = this.Count;
                if (index >= 0)
                {
                    actualCount = list.Count - index;
                    for (int i = index; i < list.Count; ++i)
                    {
                        this.Add(list[i]);
                    }
                    TotalCount += actualCount;
                    _current_page++;
                    HasMoreItems = true;
                }
            }
            else
            {
                HasMoreItems = false;
            }
            if (DataLoaded != null)
            {
                DataLoaded();
            }
            _busy = false;
            return new LoadMoreItemsResult
            {
                Count = (uint)actualCount
            };
        }
    }
}
