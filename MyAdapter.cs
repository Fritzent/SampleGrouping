using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleGrouping
{
    public class MyAdapter : RecyclerView.Adapter, ItemMoveCallback.ItemTouchHelperContract, View.IOnClickListener
    {
        #region Fields
        private Dictionary<Guid, List<HomeScreenMenuItem>> _groupingDictionary;
        private int _targetPositionToShowIndicatorGrouping;
        private bool _isIndicatorGroupingShowForTarget;
        #endregion
        #region Properties
        public int LayoutPosition { get; set; }
        public List<HomeScreenMenuItem> data;
        public Dictionary<Guid, List<HomeScreenMenuItem>> GroupDictionary
        {
            get
            {
                if (_groupingDictionary == null)
                    _groupingDictionary = new Dictionary<Guid, List<HomeScreenMenuItem>>();
                return _groupingDictionary;
            }
            set
            {
                if (_groupingDictionary != value)
                {
                    _groupingDictionary = value;
                }
            }
        }
        public int TargetPositionToShowIndicatorGrouping 
        {
            get { return _targetPositionToShowIndicatorGrouping; }
            set
            {
                if (_targetPositionToShowIndicatorGrouping != value)
                {
                    _targetPositionToShowIndicatorGrouping = value;
                }
            }
        }
        public bool IsIndicatorGroupingShowForTarget
        {
            get { return _isIndicatorGroupingShowForTarget; }
            set
            {
                if (_isIndicatorGroupingShowForTarget != value)
                {
                    _isIndicatorGroupingShowForTarget = value;
                }
            }
        }
        public int previousContainerSize { get; set; }
        public int previousItemContainerSize { get; set; }
        public MainActivity MainActivity { get; set; }
        public bool isModeEditNow { get; set; }
        public RecyclerView RecyclerView { get; set; }
        public HomeScreenMenu HomeScreenMenu { get; set; }
        public int LastPagePositionBeforeInEditMode { get; set; }
        public List<HomeScreenMenuItem> LastSavedData { get; set; }

        public string LastActionDoing { get; set; }
        public List<HomeScreenMenuItem> LastItemsUpdated { get; set; }
        public List<HomeScreenMenuItem> ItemThatPositionMoveInDeletePage { get; set; }
        public bool isPageMovedAfterDeletePage { get; set; }
        #endregion

        public MyAdapter(List<HomeScreenMenuItem> data, MainActivity mainActivity, HomeScreenMenu homeScreenMenu, RecyclerView recyclerView)
        {
            //disini sebelum masuk ke this data harus di handle sesuai posisi dulu
            //this.data = data;
            this.LoadData(data, homeScreenMenu);
            this.MainActivity = mainActivity;
            this.RecyclerView = recyclerView;
            this.HomeScreenMenu = homeScreenMenu;

        }

        public string GetUndoTitle()
        {
            string title = "";
            if (this.LastItemsUpdated != null && this.LastActionDoing == "DeleteItem")
            {
                int index = 0;
                foreach (var item in this.LastItemsUpdated)
                {
                    title += item.ItemName;
                    if (index < this.LastItemsUpdated.Count - 1)
                        title += ", ";
                    index++;
                }
                if (this.LastActionDoing == "DeleteItem")
                    title += " Deleted";
            }
            if (this.LastActionDoing == "DeletePage")
            {
                title = "Page Deleted";
            }
            return title;
        }
        public HomeScreenMenu AddPage(HomeScreenMenu homeScreenMenu) 
        {
            var checkPageSizeNow = homeScreenMenu.pageSize;
            var updateCheckPageSize = checkPageSizeNow + 1;
            homeScreenMenu.pageSize = updateCheckPageSize;

            //cari cara buat load data yg baru
            //this.LoadData(this.data, homeScreenMenu);
            this.LoadDataAfterAddPage(homeScreenMenu.pageSize);
            return homeScreenMenu;
        }
        public HomeScreenMenu DeletePage(HomeScreenMenu homeScreenMenu)
        {
            var checkPageSizeNow = homeScreenMenu.pageSize;
            var updateCheckPageSize = checkPageSizeNow - 1;
            var pagePositionNow = this.LastPagePositionBeforeInEditMode;
            homeScreenMenu.pageSize = updateCheckPageSize;
            //harus ada function yg untuk hapus data item di page yg mau di delete
            this.DeleteItemInPageDeleted(pagePositionNow, checkPageSizeNow);
            //harus ada funtion yg untuk update data itemnya
            if (this.LastPagePositionBeforeInEditMode > homeScreenMenu.pageSize)
            {
                //ini dalam kondisi harus di lempar ke page sebelum page yg dihapus
                var pageToShow = (this.LastPagePositionBeforeInEditMode - 1);
                this.LastPagePositionBeforeInEditMode = pageToShow;
                this.isPageMovedAfterDeletePage = true;
            }
            else
            {
                //kondisi dia tetap di page yg skrg 
                this.isPageMovedAfterDeletePage = false;
            }
            this.LoadDataAfterAddPage(this.LastPagePositionBeforeInEditMode);
            this.MainActivity.ShowUndo();

            return homeScreenMenu;
        }
        public void DeleteItemInPageDeleted(int pagePositionToDelete, int oldPageSize)
        {
            var lastItemPositionInPageDeleted = (12 * pagePositionToDelete);
            var startItemPositionInPageDeleted = (lastItemPositionInPageDeleted - 12);
            List<HomeScreenMenuItem> lastItemsUpdated = new List<HomeScreenMenuItem>();
            List<HomeScreenMenuItem> savedItemThatPositionMoveAfterDeletePage = new List<HomeScreenMenuItem>();

            for (int i = startItemPositionInPageDeleted; i < lastItemPositionInPageDeleted; i ++)
            {
                List<HomeScreenMenuItem> itemToDeleteFromData = this.data.Where(o => o.ItemPositionForEdit == i && o.IsDeleted == false).ToList();
                List<HomeScreenMenuItem> itemToDeleteFromLastSavedData = this.LastSavedData.Where(o => o.ItemPositionForEdit == i && o.IsDeleted == false).ToList();

                foreach (HomeScreenMenuItem item in itemToDeleteFromData)
                {
                    item.IsDeleted = true;
                    foreach (HomeScreenMenuItem itemFromSavedData in itemToDeleteFromLastSavedData)
                        itemFromSavedData.IsDeleted = item.IsDeleted;

                    lastItemsUpdated.Add(item);
                }

                this.NotifyItemChanged(i);
            }

            //update juga item yg ada di page setelah page yg skrg
            if (pagePositionToDelete < oldPageSize)
            {
                var lastItemPositionInOldPageSizeToMove = (12 * oldPageSize);
                var startItemPositionInOldPageSizeToMove = (lastItemPositionInPageDeleted);

                for (int i = startItemPositionInOldPageSizeToMove; i < lastItemPositionInOldPageSizeToMove; i++)
                {
                    List<HomeScreenMenuItem> itemToMovePositionFromLastSavedData = this.LastSavedData.Where(o => o.ItemPositionForEdit == i && o.IsDeleted == false).ToList();

                    foreach (HomeScreenMenuItem itemFromSavedData in itemToMovePositionFromLastSavedData)
                    {
                        var savedLastPosition = itemFromSavedData.ItemPositionForEdit;
                        itemFromSavedData.ItemPosition = savedLastPosition - 12;
                        //var column = 3;
                        //var row = 4;
                        //var countItemPosition = ((itemFromSavedData.ItemPositionForEdit / 12) * 12) + ((itemFromSavedData.ItemPositionForEdit % column) * row) + ((itemFromSavedData.ItemPositionForEdit % 12) / column);
                        //itemFromSavedData.ItemPosition = countItemPosition;
                        savedItemThatPositionMoveAfterDeletePage.Add(itemFromSavedData);
                    }
                    this.NotifyItemChanged(i);
                }
                //List<HomeScreenMenuItem> itemToMovePositionFromData = this.
            }

            this.LastItemsUpdated = lastItemsUpdated;
            this.ItemThatPositionMoveInDeletePage = savedItemThatPositionMoveAfterDeletePage;
            this.LastActionDoing = "DeletePage";
        }
        public void AddItems(int Position)
        {
            List<HomeScreenMenuItem> getItemsInPosition = this.data.Where(o => o.ItemPositionForEdit == Position).ToList();
            List<HomeScreenMenuItem> getItemsInPositionFromLastSavedData = this.LastSavedData.Where(o => o.ItemPositionForEdit == Position).ToList();

            if (getItemsInPosition.Count > 0 && getItemsInPositionFromLastSavedData.Count > 0)
            {
                foreach (HomeScreenMenuItem item in getItemsInPosition)
                {
                    ///item.ItemPositionForEdit = Position;
                    var column = 3;
                    var row = 4;
                    var countItemPosition = ((Position / 12) * 12) + ((Position % column) * row) + ((Position % 12) / column);
                    item.ItemPosition = countItemPosition;
                    item.ItemName = "NewItems" + item.ItemPosition;
                    item.IsDeleted = false;
                    item.ItemType = "product";
                    item.HomeScreenMenuItemId = Guid.NewGuid();
                    item.HomeScreenMenuId = this.HomeScreenMenu.homeScreenMenuId;

                    foreach (HomeScreenMenuItem itemInLastSavedData in getItemsInPositionFromLastSavedData)
                    {
                        //itemInLastSavedData.ItemPositionForEdit = item.ItemPositionForEdit;
                        itemInLastSavedData.ItemPosition = item.ItemPosition;
                        itemInLastSavedData.ItemName = item.ItemName;
                        itemInLastSavedData.IsDeleted = item.IsDeleted;
                        itemInLastSavedData.ItemType = item.ItemType;
                        itemInLastSavedData.HomeScreenMenuItemId = item.HomeScreenMenuItemId;
                        itemInLastSavedData.HomeScreenMenuId = item.HomeScreenMenuId;
                    }
                }
            }
            //var checkData = this.data;
            this.NotifyDataSetChanged();
            this.NotifyItemChanged(Position);
        }
        public void DeleteItems(int Position)
        {
            List<HomeScreenMenuItem> itemToDelete = this.data.Where(o => o.ItemPositionForEdit == Position).ToList();
            List<HomeScreenMenuItem> itemToDeleteInSavedData = this.LastSavedData.Where(o => o.ItemPositionForEdit == Position).ToList();
            List<HomeScreenMenuItem> lastItemsUpdated = new List<HomeScreenMenuItem>();

            foreach (HomeScreenMenuItem item in itemToDelete)
            {
                item.IsDeleted = true;
                lastItemsUpdated.Add(item);
                foreach (HomeScreenMenuItem itemInSavedData in itemToDeleteInSavedData)
                    itemInSavedData.IsDeleted = item.IsDeleted;
            }
            this.LastItemsUpdated = lastItemsUpdated;
            this.LastActionDoing = "DeleteItem";

            //this.LoadData(this.data, this.HomeScreenMenu);
            this.LoadDataAfterDeleteItem(this.LastPagePositionBeforeInEditMode);
            this.NotifyItemChanged(Position);
            this.MainActivity.ShowUndo();
            //this.OnPropertyChanged("ShowUndo");
        }
        public void UndoDeletedItem()
        {
            if (this.LastItemsUpdated != null)
            {
                foreach (var item in this.LastItemsUpdated)
                {
                    List<HomeScreenMenuItem> itemInData = this.data.Where(o => o.ItemPositionForEdit == item.ItemPositionForEdit && o.IsDeleted == false).ToList();
                    item.IsDeleted = false;
                    foreach (var itemToChange in itemInData)
                    {
                        itemToChange.ItemPosition = item.ItemPosition;
                        //itemToChange.ItemPositionForEdit = item.ItemPositionForEdit;
                        itemToChange.IsDeleted = item.IsDeleted;
                        itemToChange.ItemName = item.ItemName;
                        itemToChange.ItemType = item.ItemType;
                        itemToChange.ListGroupItemName = item.ListGroupItemName;
                        itemToChange.GroupName = item.GroupName;
                        itemToChange.HomeScreenMenuId = item.HomeScreenMenuId;
                        itemToChange.HomeScreenMenuItemId = item.HomeScreenMenuItemId;
                    }
                    this.NotifyItemChanged(item.ItemPositionForEdit);
                }
            }
        }
        public HomeScreenMenu UndoDeletePage(HomeScreenMenu homeScreenMenu)
        {
            var checkPageSizeNow = homeScreenMenu.pageSize;
            var updateCheckPageSize = checkPageSizeNow + 1;
            var pagePositionNow = this.LastPagePositionBeforeInEditMode;
            homeScreenMenu.pageSize = updateCheckPageSize;

            foreach (HomeScreenMenuItem itemToMoveBack in this.ItemThatPositionMoveInDeletePage)
            {
                var savedLastPosition = itemToMoveBack.ItemPositionForEdit;
                itemToMoveBack.ItemPosition = savedLastPosition + 12;
                List<HomeScreenMenuItem> itemToMoveFromLastSavedData = this.LastSavedData.Where(o => o.ItemPositionForEdit == itemToMoveBack.ItemPositionForEdit && o.IsDeleted == false).ToList();
                List<HomeScreenMenuItem> itemToMoveFromData = this.data.Where(o => o.ItemPositionForEdit == itemToMoveBack.ItemPositionForEdit && o.IsDeleted == false).ToList();

                foreach (var item in itemToMoveFromLastSavedData)
                {
                    item.ItemPosition = itemToMoveBack.ItemPosition;
                    //var column = 3;
                    //var row = 4;
                    //var countItemPosition = ((item.ItemPositionForEdit / 12) * 12) + ((item.ItemPositionForEdit % column) * row) + ((item.ItemPositionForEdit % 12) / column);
                    //item.ItemPosition = countItemPosition;
                }
                foreach (var itemData in itemToMoveFromData)
                {
                    itemData.ItemPosition = itemToMoveBack.ItemPosition;
                    //var column = 3;
                    //var row = 4;
                    //var countItemPosition = ((itemData.ItemPositionForEdit / 12) * 12) + ((itemData.ItemPositionForEdit % column) * row) + ((itemData.ItemPositionForEdit % 12) / column);
                    //itemData.ItemPosition = countItemPosition;
                }
            }

            foreach (HomeScreenMenuItem itemToRestore in this.LastItemsUpdated)
            {
                List<HomeScreenMenuItem> itemInLastSaved = this.LastSavedData.Where(o => o.ItemPositionForEdit == itemToRestore.ItemPositionForEdit && o.HomeScreenMenuItemId == itemToRestore.HomeScreenMenuItemId && o.IsDeleted == true).ToList();
                if (itemInLastSaved != null)
                {
                    foreach (var item in itemInLastSaved)
                        item.IsDeleted = false;
                }
            }

            if (this.isPageMovedAfterDeletePage)
            {
                var saveLastPagePosition = this.LastPagePositionBeforeInEditMode;
                this.LastPagePositionBeforeInEditMode = saveLastPagePosition + 1;
                this.LoadDataAfterAddPage(this.LastPagePositionBeforeInEditMode);
            }
            else
            {
                this.LoadDataAfterAddPage(this.LastPagePositionBeforeInEditMode);
            }

            return homeScreenMenu;
        }
        public void LoadData(List<HomeScreenMenuItem> data, HomeScreenMenu homeScreenMenu)
        {
            var pageSize = homeScreenMenu.pageSize;
            var itemCount = 12 * pageSize;

            List<HomeScreenMenuItem> itemGenerated = new List<HomeScreenMenuItem>();

            for (int i = 0; i < itemCount; i ++)
            {
                List<HomeScreenMenuItem> findItem = data.Where(o => o.ItemPosition == i && o.IsDeleted == false).ToList();
                if (findItem.Count > 0)
                {
                    foreach (HomeScreenMenuItem item in findItem)
                    {
                        if (item != null)
                        {
                            if (!string.IsNullOrEmpty(item.GroupName))
                            {
                                List<HomeScreenMenuItem> getGroupItem = data.Where(o => o.ItemPosition == item.ItemPosition).ToList();
                                foreach (HomeScreenMenuItem eachItem in getGroupItem)
                                {
                                    if (!this.GroupDictionary.ContainsKey(eachItem.GroupingId))
                                        this.GroupDictionary.Add(eachItem.GroupingId, getGroupItem);
                                }
                                var LastItemInThisGroup = getGroupItem.Last();

                                if (item.HomeScreenMenuItemId == LastItemInThisGroup.HomeScreenMenuItemId)
                                {
                                    item.ListGroupItemName = new List<string>();

                                    List<HomeScreenMenuItem> itemsInDictionary = this.GroupDictionary[item.GroupingId];

                                    foreach (HomeScreenMenuItem itemInDictionary in itemsInDictionary)
                                    {
                                        if (itemInDictionary.HomeScreenMenuItemId != item.HomeScreenMenuItemId)
                                        {
                                            if (itemInDictionary.ItemType == "product")
                                            {
                                                if (itemInDictionary.ItemName != null)
                                                    item.ListGroupItemName.Add(itemInDictionary.ItemName);
                                            }
                                        }
                                        else
                                        {
                                            if (item.ItemType == "product")
                                            {
                                                if (item.ItemName != null)
                                                    item.ListGroupItemName.Add(item.ItemName);
                                            }
                                        }
                                    }
                                    itemGenerated.Add(item);
                                }
                            }
                            else
                            {
                                itemGenerated.Add(item);
                            }
                        }
                        else
                        {
                            HomeScreenMenuItem emptyItem = new HomeScreenMenuItem();
                            emptyItem.ItemPosition = i;
                            //emptyItem.ItemName = "New Item" + (i + 1);
                            emptyItem.HomeScreenMenuItemId = Guid.NewGuid();
                            emptyItem.HomeScreenMenuId = homeScreenMenu.homeScreenMenuId;

                            itemGenerated.Add(emptyItem);
                        }
                    }
                }
                else
                {
                    HomeScreenMenuItem emptyItem = new HomeScreenMenuItem();
                    emptyItem.ItemPosition = i;
                    //emptyItem.ItemName = "New Item" + i;
                    emptyItem.HomeScreenMenuItemId = Guid.NewGuid();
                    //emptyItem.ItemType = "product";
                    emptyItem.HomeScreenMenuId = homeScreenMenu.homeScreenMenuId;

                    itemGenerated.Add(emptyItem);
                }
            }
            this.data = itemGenerated;
        }
        public void LoadDataAfterChangeMode(int pagePosition)
        {
            this.LastPagePositionBeforeInEditMode = pagePosition;
            var data = this.data;
            var homeScreenMenu = this.HomeScreenMenu;

            var y = pagePosition * 12;
            var x = y - 12;

            List<HomeScreenMenuItem> itemGenerated = new List<HomeScreenMenuItem>();

            for (int i = x; i < y; i++)
            {
                List<HomeScreenMenuItem> findItem = data.Where(o => o.ItemPositionForEdit == i && o.IsDeleted == false).ToList();
                if (findItem.Count > 0)
                {
                    foreach (HomeScreenMenuItem item in findItem)
                    {
                        if (item != null)
                        {
                            if (!string.IsNullOrEmpty(item.GroupName))
                            {
                                List<HomeScreenMenuItem> getGroupItem = data.Where(o => o.ItemPositionForEdit == item.ItemPositionForEdit).ToList();
                                foreach (HomeScreenMenuItem eachItem in getGroupItem)
                                {
                                    if (!this.GroupDictionary.ContainsKey(eachItem.GroupingId))
                                        this.GroupDictionary.Add(eachItem.GroupingId, getGroupItem);
                                }
                                var LastItemInThisGroup = getGroupItem.Last();

                                if (item.HomeScreenMenuItemId == LastItemInThisGroup.HomeScreenMenuItemId)
                                {
                                    item.ListGroupItemName = new List<string>();

                                    List<HomeScreenMenuItem> itemsInDictionary = this.GroupDictionary[item.GroupingId];

                                    foreach (HomeScreenMenuItem itemInDictionary in itemsInDictionary)
                                    {
                                        if (itemInDictionary.HomeScreenMenuItemId != item.HomeScreenMenuItemId)
                                        {
                                            if (itemInDictionary.ItemType == "product")
                                            {
                                                if (itemInDictionary.ItemName != null)
                                                    item.ListGroupItemName.Add(itemInDictionary.ItemName);
                                            }
                                        }
                                        else
                                        {
                                            if (item.ItemType == "product")
                                            {
                                                if (item.ItemName != null)
                                                    item.ListGroupItemName.Add(item.ItemName);
                                            }
                                        }
                                    }
                                    itemGenerated.Add(item);
                                }
                            }
                            else
                            {
                                itemGenerated.Add(item);
                            }
                        }
                        else
                        {
                            HomeScreenMenuItem emptyItem = new HomeScreenMenuItem();
                            //emptyItem.ItemPositionForEdit = i;
                            var column = 3;
                            var row = 4;
                            var countItemPosition = ((i / 12) * 12) + ((i % column) * row) + ((i % 12) / column);
                            emptyItem.ItemPosition = countItemPosition;
                            //emptyItem.ItemName = "New Item" + (i + 1);
                            emptyItem.HomeScreenMenuItemId = Guid.NewGuid();
                            emptyItem.HomeScreenMenuId = homeScreenMenu.homeScreenMenuId;

                            itemGenerated.Add(emptyItem);
                        }
                    }
                }
                else
                {
                    HomeScreenMenuItem emptyItem = new HomeScreenMenuItem();
                    //emptyItem.ItemPositionForEdit = i;
                    var column = 3;
                    var row = 4;
                    var countItemPosition = ((i / 12) * 12) + ((i % column) * row) + ((i % 12) / column);
                    emptyItem.ItemPosition = countItemPosition;
                    //emptyItem.ItemName = "New Item" + i;
                    emptyItem.HomeScreenMenuItemId = Guid.NewGuid();
                    //emptyItem.ItemType = "product";
                    emptyItem.HomeScreenMenuId = homeScreenMenu.homeScreenMenuId;

                    itemGenerated.Add(emptyItem);
                }
            }
            this.LastSavedData = data;
            this.data = itemGenerated;
        }
        public void LoadDataAfterDeleteItem (int pagePosition)
        {
            this.LastPagePositionBeforeInEditMode = pagePosition;
            var data = this.data;
            var homeScreenMenu = this.HomeScreenMenu;

            var y = pagePosition * 12;
            var x = y - 12;

            List<HomeScreenMenuItem> itemGenerated = new List<HomeScreenMenuItem>();

            for (int i = x; i < y; i++)
            {
                List<HomeScreenMenuItem> findItem = data.Where(o => o.ItemPositionForEdit == i && o.IsDeleted == false).ToList();
                if (findItem.Count > 0)
                {
                    foreach (HomeScreenMenuItem item in findItem)
                    {
                        if (item != null)
                        {
                            if (!string.IsNullOrEmpty(item.GroupName))
                            {
                                List<HomeScreenMenuItem> getGroupItem = data.Where(o => o.ItemPositionForEdit == item.ItemPositionForEdit).ToList();
                                foreach (HomeScreenMenuItem eachItem in getGroupItem)
                                {
                                    if (!this.GroupDictionary.ContainsKey(eachItem.GroupingId))
                                        this.GroupDictionary.Add(eachItem.GroupingId, getGroupItem);
                                }
                                var LastItemInThisGroup = getGroupItem.Last();

                                if (item.HomeScreenMenuItemId == LastItemInThisGroup.HomeScreenMenuItemId)
                                {
                                    item.ListGroupItemName = new List<string>();

                                    List<HomeScreenMenuItem> itemsInDictionary = this.GroupDictionary[item.GroupingId];

                                    foreach (HomeScreenMenuItem itemInDictionary in itemsInDictionary)
                                    {
                                        if (itemInDictionary.HomeScreenMenuItemId != item.HomeScreenMenuItemId)
                                        {
                                            if (itemInDictionary.ItemType == "product")
                                            {
                                                if (itemInDictionary.ItemName != null)
                                                    item.ListGroupItemName.Add(itemInDictionary.ItemName);
                                            }
                                        }
                                        else
                                        {
                                            if (item.ItemType == "product")
                                            {
                                                if (item.ItemName != null)
                                                    item.ListGroupItemName.Add(item.ItemName);
                                            }
                                        }
                                    }
                                    itemGenerated.Add(item);
                                }
                            }
                            else
                            {
                                itemGenerated.Add(item);
                            }
                        }
                        else
                        {
                            HomeScreenMenuItem emptyItem = new HomeScreenMenuItem();
                            //emptyItem.ItemPositionForEdit = i;
                            var column = 3;
                            var row = 4;
                            var countItemPosition = ((i / 12) * 12) + ((i % column) * row) + ((i % 12) / column);
                            emptyItem.ItemPosition = countItemPosition;
                            //emptyItem.ItemName = "New Item" + (i + 1);
                            emptyItem.HomeScreenMenuItemId = Guid.NewGuid();
                            emptyItem.HomeScreenMenuId = homeScreenMenu.homeScreenMenuId;

                            itemGenerated.Add(emptyItem);
                        }
                    }
                }
                else
                {
                    HomeScreenMenuItem emptyItem = new HomeScreenMenuItem();
                    //emptyItem.ItemPositionForEdit = i;
                    var column = 3;
                    var row = 4;
                    var countItemPosition = ((i / 12) * 12) + ((i % column) * row) + ((i % 12) / column);
                    emptyItem.ItemPosition = countItemPosition;
                    //emptyItem.ItemName = "New Item" + i;
                    emptyItem.HomeScreenMenuItemId = Guid.NewGuid();
                    //emptyItem.ItemType = "product";
                    emptyItem.HomeScreenMenuId = homeScreenMenu.homeScreenMenuId;

                    itemGenerated.Add(emptyItem);
                }
            }
            this.data = itemGenerated;
        }
        public bool LoadDataAfterGrouping()
        {
            List<HomeScreenMenuItem> dataSource = this.data.ToList();
            List<HomeScreenMenuItem> newDataGenerated = new List<HomeScreenMenuItem>();

            for (int x = 0; x < dataSource.Count(); x++)
            {
                //List<HomeScreenMenuItem> findItemInResult = homeScreenItemResult.Where(o => o.Position == x && o.IsDeleted == false).ToList();
                List<HomeScreenMenuItem> findItemInDataSource = this.data.Where(o => o.ItemPositionForEdit == x).ToList();

                if (findItemInDataSource.Count() > 0)
                {
                    foreach (HomeScreenMenuItem item in findItemInDataSource)
                    {
                        if (item != null)
                        {
                            if (!string.IsNullOrEmpty(item.GroupName))
                            {
                                //List<HomeScreenMenuItem> getGroupItem = homeScreenItemResult.Where(o => o.Position == item.Position).ToList();
                                List<HomeScreenMenuItem> getGroupItemInDataSource = dataSource.Where(o => o.ItemPositionForEdit == item.ItemPositionForEdit).ToList();

                                foreach (var eachItem in getGroupItemInDataSource)
                                {
                                    if (!this.GroupDictionary.ContainsKey(eachItem.GroupingId))
                                        this.GroupDictionary.Add(eachItem.GroupingId, getGroupItemInDataSource);
                                    if (this.GroupDictionary.ContainsKey(eachItem.GroupingId))
                                    {
                                        if (!this.GroupDictionary[eachItem.GroupingId].Contains(eachItem))
                                            this.GroupDictionary[eachItem.GroupingId].Add(eachItem);
                                    }
                                }

                                var LastItemInThisGroup = getGroupItemInDataSource.Last();

                                if (item.HomeScreenMenuItemId == LastItemInThisGroup.HomeScreenMenuItemId)
                                {
                                    item.ListGroupItemName = new List<string>();

                                    List<HomeScreenMenuItem> itemsInDictionary = this.GroupDictionary[item.GroupingId];

                                    foreach (HomeScreenMenuItem itemInDictionary in itemsInDictionary)
                                    {
                                        if (itemInDictionary.HomeScreenMenuItemId != item.HomeScreenMenuItemId)
                                        {
                                            if (itemInDictionary.ItemType == "product")
                                            {
                                                if (itemInDictionary.ItemName != null)
                                                    item.ListGroupItemName.Add(itemInDictionary.ItemName);
                                            }
                                        }
                                        else
                                        {
                                            if (item.ItemType == "product")
                                            {
                                                if (item.ItemName != null)
                                                    item.ListGroupItemName.Add(item.ItemName);
                                            }
                                        }
                                    }
                                    newDataGenerated.Add(item);
                                }
                                else
                                {

                                }
                            }
                            else
                                newDataGenerated.Add(item);
                        }
                    }
                }
                else
                {
                    HomeScreenMenuItem emptyItem = new HomeScreenMenuItem();
                    //emptyItem.ItemPositionForEdit = x;
                    var column = 3;
                    var row = 4;
                    var countItemPosition = ((x / 12) * 12) + ((x % column) * row) + ((x % 12) / column);
                    emptyItem.ItemPosition = countItemPosition;
                    //emptyItem.ItemName = "New Item" + i;
                    emptyItem.HomeScreenMenuItemId = Guid.NewGuid();
                    //emptyItem.ItemType = "product";
                    emptyItem.HomeScreenMenuId = this.HomeScreenMenu.homeScreenMenuId;

                    newDataGenerated.Add(emptyItem);
                }
            }
            this.data = newDataGenerated;
            this.NotifyDataSetChanged();

            return true;
        }
        public void LoadDataAfterAddPage(int newPagePosition)
        {
            this.LastPagePositionBeforeInEditMode = newPagePosition;
            var data = this.LastSavedData;

            var homeScreenMenu = this.HomeScreenMenu;

            var y = newPagePosition * 12;
            var x = y - 12;

            List<HomeScreenMenuItem> itemGenerated = new List<HomeScreenMenuItem>();

            for (int i = x; i < y; i++)
            {
                List<HomeScreenMenuItem> findItem = data.Where(o => o.ItemPositionForEdit == i && o.IsDeleted == false).ToList();
                if (findItem.Count > 0)
                {
                    foreach (HomeScreenMenuItem item in findItem)
                    {
                        if (item != null)
                        {
                            if (!string.IsNullOrEmpty(item.GroupName))
                            {
                                List<HomeScreenMenuItem> getGroupItem = data.Where(o => o.ItemPositionForEdit == item.ItemPositionForEdit).ToList();
                                foreach (HomeScreenMenuItem eachItem in getGroupItem)
                                {
                                    if (!this.GroupDictionary.ContainsKey(eachItem.GroupingId))
                                        this.GroupDictionary.Add(eachItem.GroupingId, getGroupItem);
                                }
                                var LastItemInThisGroup = getGroupItem.Last();

                                if (item.HomeScreenMenuItemId == LastItemInThisGroup.HomeScreenMenuItemId)
                                {
                                    item.ListGroupItemName = new List<string>();

                                    List<HomeScreenMenuItem> itemsInDictionary = this.GroupDictionary[item.GroupingId];

                                    foreach (HomeScreenMenuItem itemInDictionary in itemsInDictionary)
                                    {
                                        if (itemInDictionary.HomeScreenMenuItemId != item.HomeScreenMenuItemId)
                                        {
                                            if (itemInDictionary.ItemType == "product")
                                            {
                                                if (itemInDictionary.ItemName != null)
                                                    item.ListGroupItemName.Add(itemInDictionary.ItemName);
                                            }
                                        }
                                        else
                                        {
                                            if (item.ItemType == "product")
                                            {
                                                if (item.ItemName != null)
                                                    item.ListGroupItemName.Add(item.ItemName);
                                            }
                                        }
                                    }
                                    itemGenerated.Add(item);
                                }
                            }
                            else
                            {
                                itemGenerated.Add(item);
                            }
                        }
                        else
                        {
                            HomeScreenMenuItem emptyItem = new HomeScreenMenuItem();
                            //emptyItem.ItemPosition = i;
                            //emptyItem.ItemPositionForEdit = i;
                            var column = 3;
                            var row = 4;
                            var countItemPosition = ((i / 12) * 12) + ((i % column) * row) + ((i % 12) / column);
                            emptyItem.ItemPosition = countItemPosition;
                            //emptyItem.ItemName = "New Item" + (i + 1);
                            emptyItem.HomeScreenMenuItemId = Guid.NewGuid();
                            emptyItem.HomeScreenMenuId = homeScreenMenu.homeScreenMenuId;

                            itemGenerated.Add(emptyItem);
                        }
                    }
                }
                else
                {
                    HomeScreenMenuItem emptyItem = new HomeScreenMenuItem();
                    //emptyItem.ItemPosition = i;
                    //emptyItem.ItemPositionForEdit = i;
                    var column = 3;
                    var row = 4;
                    var countItemPosition = ((i / 12) * 12) + ((i % column) * row) + ((i % 12) / column);
                    emptyItem.ItemPosition = countItemPosition;
                    //emptyItem.ItemName = "New Item" + i;
                    emptyItem.HomeScreenMenuItemId = Guid.NewGuid();
                    //emptyItem.ItemType = "product";
                    emptyItem.HomeScreenMenuId = homeScreenMenu.homeScreenMenuId;

                    itemGenerated.Add(emptyItem);
                }
            }
            this.LastSavedData = data;
            this.data = itemGenerated;
        }
        public void IndicatorOnEditModeClick(string mode)
        {
            if (mode != null)
            {
                var homeScreenMenu = this.HomeScreenMenu;
                var checkPageSize = homeScreenMenu.pageSize;

                List<HomeScreenMenuItem> itemGenerated = new List<HomeScreenMenuItem>();

                if (mode == "next")
                {
                    if (this.LastPagePositionBeforeInEditMode != checkPageSize)
                    {
                        //dia bukan page terakhir
                        var newPagePosition = this.LastPagePositionBeforeInEditMode + 1;
                        var itemCount = 12 * newPagePosition;
                        var lastItemPositionInNewPage = itemCount;
                        var startItemPositionInNewPage = ((lastItemPositionInNewPage - 11)-1);

                        for (int i = startItemPositionInNewPage; i < itemCount; i++)
                        {
                            List<HomeScreenMenuItem> findItem = this.LastSavedData.Where(o => o.ItemPositionForEdit == i && o.IsDeleted == false).ToList();
                            if (findItem.Count > 0)
                            {
                                foreach (HomeScreenMenuItem item in findItem)
                                {
                                    if (item != null)
                                    {
                                        if (!string.IsNullOrEmpty(item.GroupName))
                                        {
                                            List<HomeScreenMenuItem> getGroupItem = this.LastSavedData.Where(o => o.ItemPositionForEdit == item.ItemPositionForEdit && o.IsDeleted == false).ToList();
                                            foreach (HomeScreenMenuItem eachItem in getGroupItem)
                                            {
                                                if (!this.GroupDictionary.ContainsKey(eachItem.GroupingId))
                                                    this.GroupDictionary.Add(eachItem.GroupingId, getGroupItem);
                                            }
                                            var LastItemInThisGroup = getGroupItem.Last();

                                            if (item.HomeScreenMenuItemId == LastItemInThisGroup.HomeScreenMenuItemId)
                                            {
                                                item.ListGroupItemName = new List<string>();

                                                List<HomeScreenMenuItem> itemsInDictionary = this.GroupDictionary[item.GroupingId];

                                                foreach (HomeScreenMenuItem itemInDictionary in itemsInDictionary)
                                                {
                                                    if (itemInDictionary.HomeScreenMenuItemId != item.HomeScreenMenuItemId)
                                                    {
                                                        if (itemInDictionary.ItemType == "product")
                                                        {
                                                            if (itemInDictionary.ItemName != null)
                                                                item.ListGroupItemName.Add(itemInDictionary.ItemName);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (item.ItemType == "product")
                                                        {
                                                            if (item.ItemName != null)
                                                                item.ListGroupItemName.Add(item.ItemName);
                                                        }
                                                    }
                                                }
                                                itemGenerated.Add(item);
                                            }
                                        }
                                        else
                                        {
                                            itemGenerated.Add(item);
                                        }
                                    }
                                    else
                                    {
                                        HomeScreenMenuItem emptyItem = new HomeScreenMenuItem();
                                        //emptyItem.ItemPosition = i;
                                        //emptyItem.ItemPositionForEdit = i;
                                        var column = 3;
                                        var row = 4;
                                        var countItemPosition = ((i / 12) * 12) + ((i % column) * row) + ((i % 12) / column);
                                        emptyItem.ItemPosition = countItemPosition;
                                        //emptyItem.ItemName = "New Item" + (i + 1);
                                        emptyItem.HomeScreenMenuItemId = Guid.NewGuid();
                                        emptyItem.HomeScreenMenuId = homeScreenMenu.homeScreenMenuId;

                                        itemGenerated.Add(emptyItem);
                                    }
                                }
                            }
                            else
                            {
                                HomeScreenMenuItem emptyItem = new HomeScreenMenuItem();
                                //emptyItem.ItemPosition = i;
                                //emptyItem.ItemPositionForEdit = i;
                                var column = 3;
                                var row = 4;
                                var countItemPosition = ((i / 12) * 12) + ((i % column) * row) + ((i % 12) / column);
                                emptyItem.ItemPosition = countItemPosition;
                                //emptyItem.ItemName = "New Item" + i;
                                emptyItem.HomeScreenMenuItemId = Guid.NewGuid();
                                //emptyItem.ItemType = "product";
                                emptyItem.HomeScreenMenuId = homeScreenMenu.homeScreenMenuId;

                                itemGenerated.Add(emptyItem);
                            }
                        }

                        this.data = itemGenerated;
                        this.LastPagePositionBeforeInEditMode = newPagePosition;
                    }
                    else
                    {
                        //kosongin dulu aja
                    }
                }
                else
                {
                    //disini untuk check page sebelumnya
                    if (this.LastPagePositionBeforeInEditMode != 1)
                    {
                        var newPagePosition = this.LastPagePositionBeforeInEditMode - 1;
                        var itemCount = 12 * newPagePosition;
                        var lastItemPositionInNewPage = itemCount;
                        var startItemPositionInNewPage = ((lastItemPositionInNewPage - 11) - 1);

                        for (int i = startItemPositionInNewPage; i < itemCount; i++)
                        {
                            List<HomeScreenMenuItem> findItem = this.LastSavedData.Where(o => o.ItemPositionForEdit == i && o.IsDeleted == false).ToList();
                            if (findItem.Count > 0)
                            {
                                foreach (HomeScreenMenuItem item in findItem)
                                {
                                    if (item != null)
                                    {
                                        if (!string.IsNullOrEmpty(item.GroupName))
                                        {
                                            List<HomeScreenMenuItem> getGroupItem = this.LastSavedData.Where(o => o.ItemPositionForEdit == item.ItemPositionForEdit && o.IsDeleted == false).ToList();
                                            foreach (HomeScreenMenuItem eachItem in getGroupItem)
                                            {
                                                if (!this.GroupDictionary.ContainsKey(eachItem.GroupingId))
                                                    this.GroupDictionary.Add(eachItem.GroupingId, getGroupItem);
                                            }
                                            var LastItemInThisGroup = getGroupItem.Last();

                                            if (item.HomeScreenMenuItemId == LastItemInThisGroup.HomeScreenMenuItemId)
                                            {
                                                item.ListGroupItemName = new List<string>();

                                                List<HomeScreenMenuItem> itemsInDictionary = this.GroupDictionary[item.GroupingId];

                                                foreach (HomeScreenMenuItem itemInDictionary in itemsInDictionary)
                                                {
                                                    if (itemInDictionary.HomeScreenMenuItemId != item.HomeScreenMenuItemId)
                                                    {
                                                        if (itemInDictionary.ItemType == "product")
                                                        {
                                                            if (itemInDictionary.ItemName != null)
                                                                item.ListGroupItemName.Add(itemInDictionary.ItemName);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (item.ItemType == "product")
                                                        {
                                                            if (item.ItemName != null)
                                                                item.ListGroupItemName.Add(item.ItemName);
                                                        }
                                                    }
                                                }
                                                itemGenerated.Add(item);
                                            }
                                        }
                                        else
                                        {
                                            itemGenerated.Add(item);
                                        }
                                    }
                                    else
                                    {
                                        HomeScreenMenuItem emptyItem = new HomeScreenMenuItem();
                                        //emptyItem.ItemPosition = i;
                                        //emptyItem.ItemPositionForEdit = i;
                                        var column = 3;
                                        var row = 4;
                                        var countItemPosition = ((i / 12) * 12) + ((i % column) * row) + ((i % 12) / column);
                                        emptyItem.ItemPosition = countItemPosition;
                                        //emptyItem.ItemName = "New Item" + (i + 1);
                                        emptyItem.HomeScreenMenuItemId = Guid.NewGuid();
                                        emptyItem.HomeScreenMenuId = homeScreenMenu.homeScreenMenuId;

                                        itemGenerated.Add(emptyItem);
                                    }
                                }
                            }
                            else
                            {
                                HomeScreenMenuItem emptyItem = new HomeScreenMenuItem();
                                //emptyItem.ItemPosition = i;
                                //emptyItem.ItemPositionForEdit = i;
                                var column = 3;
                                var row = 4;
                                var countItemPosition = ((i / 12) * 12) + ((i % column) * row) + ((i % 12) / column);
                                emptyItem.ItemPosition = countItemPosition;
                                //emptyItem.ItemName = "New Item" + i;
                                emptyItem.HomeScreenMenuItemId = Guid.NewGuid();
                                //emptyItem.ItemType = "product";
                                emptyItem.HomeScreenMenuId = homeScreenMenu.homeScreenMenuId;

                                itemGenerated.Add(emptyItem);
                            }
                        }

                        this.data = itemGenerated;
                        this.LastPagePositionBeforeInEditMode = newPagePosition;
                    }
                    else
                    {
                        //kosingin dulu aja
                    }
                }
            }
        }
        public void SetDataToShowGroupingIndicator (int TargetPosition, bool IsTargetIndicatorGroupingShowed)
        {
            this.TargetPositionToShowIndicatorGrouping = TargetPosition;
            this.IsIndicatorGroupingShowForTarget = IsTargetIndicatorGroupingShowed;
        }
        public int MarginToLinearLayout { get; set; }
        public int MarginCardView { get; set; }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            MyViewHolder h = holder as MyViewHolder;

            HomeScreenMenuItem itemProduct = data[position] as HomeScreenMenuItem;

            if (h != null)
            {

                if (h.NonGroupItemSection != null)
                {
                    ViewGroup NonGroupItemSection = h.NonGroupItemSection as ViewGroup;
                    if (NonGroupItemSection != null)
                        NonGroupItemSection.Visibility = ViewStates.Gone;
                }
                if (h.NonGroupItemName != null)
                {
                    TextView NonGroupItemName = h.NonGroupItemName as TextView;
                    if (NonGroupItemName != null)
                    {
                        NonGroupItemName.Text = "";
                        NonGroupItemName.Visibility = ViewStates.Gone;
                    }
                }
                if (h.GroupItemSection != null)
                {
                    ViewGroup GroupItemSection = h.GroupItemSection as ViewGroup;
                    if (GroupItemSection != null)
                        GroupItemSection.Visibility = ViewStates.Gone;
                }
                if (h.GroupItemItemName1 != null)
                {
                    TextView GroupItemItemName1 = h.GroupItemItemName1 as TextView;
                    if (GroupItemItemName1 != null)
                    {
                        GroupItemItemName1.Text = "";
                        GroupItemItemName1.Visibility = ViewStates.Gone;
                    }
                }
                if (h.GroupItemItemName2 != null)
                {
                    TextView GroupItemItemName2 = h.GroupItemItemName2 as TextView;
                    if (GroupItemItemName2 != null)
                    {
                        GroupItemItemName2.Text = "";
                        GroupItemItemName2.Visibility = ViewStates.Gone;
                    }
                }
                if (h.GroupItemItemName3 != null)
                {
                    TextView GroupItemItemName3 = h.GroupItemItemName3 as TextView;
                    if (GroupItemItemName3 != null)
                    {
                        GroupItemItemName3.Text = "";
                        GroupItemItemName3.Visibility = ViewStates.Gone;
                    }
                }
                if (h.GroupItemItemName4 != null)
                {
                    TextView GroupItemItemName4 = h.GroupItemItemName4 as TextView;
                    if (GroupItemItemName4 != null)
                    {
                        GroupItemItemName4.Text = "";
                        GroupItemItemName4.Visibility = ViewStates.Gone;
                    }
                }
                if (h.DeleteItems != null)
                {
                    FrameLayout DeleteItems = h.DeleteItems as FrameLayout;
                    if (DeleteItems != null)
                    {
                        DeleteItems.Visibility = ViewStates.Gone;
                        DeleteItems.SetOnClickListener(null);
                    }
                }
                if (h.HomeScreenEmptyProduct != null)
                {
                    FrameLayout HomeScreenEmptyProduct = h.HomeScreenEmptyProduct as FrameLayout;
                    if (HomeScreenEmptyProduct != null)
                        HomeScreenEmptyProduct.Visibility = ViewStates.Gone;
                }
                if (h.HomeScreenEmptyProductNotOnEdit != null)
                {
                    LinearLayout HomeScreenEmptyProductNotOnEdit = h.HomeScreenEmptyProductNotOnEdit as LinearLayout;
                    if (HomeScreenEmptyProductNotOnEdit != null)
                        HomeScreenEmptyProductNotOnEdit.Visibility = ViewStates.Gone;
                }
                if (h.HomeScreenEmptyProductOnEdit != null)
                {
                    FrameLayout HomeScreenEmptyProductOnEdit = h.HomeScreenEmptyProductOnEdit as FrameLayout;
                    if (HomeScreenEmptyProductOnEdit != null)
                    {
                        HomeScreenEmptyProductOnEdit.Visibility = ViewStates.Gone;
                        HomeScreenEmptyProductOnEdit.SetOnClickListener(null);
                    }

                }
                if (h.RelativeContainer != null)
                {
                    RelativeLayout RelativeContainer = h.RelativeContainer as RelativeLayout;
                    if (RelativeContainer != null)
                        RelativeContainer.Visibility = ViewStates.Gone;
                }
                if (h.CardContainer != null)
                {
                    AndroidX.CardView.Widget.CardView CardContainer = h.CardContainer as AndroidX.CardView.Widget.CardView;
                    if (CardContainer != null)
                        CardContainer.Visibility = ViewStates.Gone;
                }
            }
            //disini handle kalau dia type yg grouping munculin yg bagian grouping
            if (!string.IsNullOrEmpty(itemProduct.ItemType))
            {
                if (!string.IsNullOrEmpty(itemProduct.GroupName) || itemProduct.ListGroupItemName != null)
                {
                    if (h.GroupItemSection != null)
                    {
                        ViewGroup GroupItemSection = h.GroupItemSection as ViewGroup;
                        if (GroupItemSection != null)
                            GroupItemSection.Visibility = ViewStates.Visible;
                        //ini untuk handle berapa jumlah nama yg dimunculin dalam item group sectionnya
                        int countGroupItemNameInListGroupItemName = itemProduct.ListGroupItemName.Count();
                        if (itemProduct.ListGroupItemName.Count() > 4)
                            countGroupItemNameInListGroupItemName = 4;

                        if (countGroupItemNameInListGroupItemName != 0)
                        {
                            for (int i = 1; i <= countGroupItemNameInListGroupItemName; i++)
                            {
                                if (i == 1)
                                {
                                    if (h.GroupItemItemName1 != null)
                                    {
                                        TextView GroupItemName = h.GroupItemItemName1 as TextView;
                                        if (GroupItemName != null)
                                        {
                                            if (itemProduct.ListGroupItemName[i - 1] != null)
                                            {
                                                GroupItemName.Text = itemProduct.ListGroupItemName[i - 1];
                                                GroupItemName.Visibility = ViewStates.Visible;
                                            }
                                        }
                                    }
                                }
                                if (i == 2)
                                {
                                    if (h.GroupItemItemName2 != null)
                                    {
                                        TextView GroupItemName = h.GroupItemItemName2 as TextView;
                                        if (GroupItemName != null)
                                        {
                                            if (itemProduct.ListGroupItemName[i - 1] != null)
                                            {
                                                GroupItemName.Text = itemProduct.ListGroupItemName[i - 1];
                                                GroupItemName.Visibility = ViewStates.Visible;
                                            }
                                        }
                                    }
                                }
                                if (i == 3)
                                {
                                    if (h.GroupItemItemName3 != null)
                                    {
                                        TextView GroupItemName = h.GroupItemItemName3 as TextView;
                                        if (GroupItemName != null)
                                        {
                                            if (itemProduct.ListGroupItemName[i - 1] != null)
                                            {
                                                GroupItemName.Text = itemProduct.ListGroupItemName[i - 1];
                                                GroupItemName.Visibility = ViewStates.Visible;
                                            }
                                        }
                                    }
                                }
                                if (i == 4)
                                {
                                    if (h.GroupItemItemName4 != null)
                                    {
                                        TextView GroupItemName = h.GroupItemItemName4 as TextView;
                                        if (GroupItemName != null)
                                        {
                                            if (itemProduct.ListGroupItemName[i - 1] != null)
                                            {
                                                GroupItemName.Text = itemProduct.ListGroupItemName[i - 1];
                                                GroupItemName.Visibility = ViewStates.Visible;
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
            }
            //disini handle kalau dia type gk grouping munculin yg bagian gk grouping
            if (!string.IsNullOrEmpty(itemProduct.ItemType))
            {
                if (string.IsNullOrEmpty(itemProduct.GroupName) && itemProduct.ListGroupItemName == null)
                {
                    if (h.NonGroupItemSection != null)
                    {
                        ViewGroup NonGroupItemSection = h.NonGroupItemSection as ViewGroup;
                        if (NonGroupItemSection != null)
                            NonGroupItemSection.Visibility = ViewStates.Visible;
                    }
                    if (h.NonGroupItemName != null)
                    {
                        TextView NonGroupItemName = h.NonGroupItemName as TextView;
                        if (NonGroupItemName != null)
                        {
                            NonGroupItemName.Text = itemProduct.ItemName;
                            NonGroupItemName.Visibility = ViewStates.Visible;
                        }

                    }
                }
            }
            //disini buat handle tampilan item yg itemtypenya gk ada
            if (string.IsNullOrEmpty(itemProduct.ItemType))
            {
                if (h.HomeScreenEmptyProduct != null)
                {
                    FrameLayout HomeScreenEmptyProduct = h.HomeScreenEmptyProduct as FrameLayout;
                    if (HomeScreenEmptyProduct != null)
                        HomeScreenEmptyProduct.Visibility = ViewStates.Visible;
                }

                if (this.isModeEditNow)
                {
                    //disini munculin yg kosong ada tanda plus
                    if (h.HomeScreenEmptyProductOnEdit != null)
                    {
                        FrameLayout HomeScreenEmptyProductOnEdit = h.HomeScreenEmptyProductOnEdit as FrameLayout;
                        if (HomeScreenEmptyProductOnEdit != null)
                        {
                            HomeScreenEmptyProductOnEdit.Visibility = ViewStates.Visible;
                            HomeScreenEmptyProductOnEdit.Tag = itemProduct.ItemPositionForEdit;
                            HomeScreenEmptyProductOnEdit.SetOnClickListener(this);
                        }
                    }
                }
                else
                {
                    //disini munculin yg kosong gk ada tanda plus
                    if (h.HomeScreenEmptyProductNotOnEdit != null)
                    {
                        LinearLayout HomeScreenEmptyProductNotOnEdit = h.HomeScreenEmptyProductNotOnEdit as LinearLayout;
                        if (HomeScreenEmptyProductNotOnEdit != null)
                            HomeScreenEmptyProductNotOnEdit.Visibility = ViewStates.Visible;
                    }
                }
            }
            //disini untuk munculin tanda x di edit modenya
            if (this.isModeEditNow)
            {
                int fragmentHeight = this.RecyclerView.Height;
                int cardElevation = dpToPx((int)4);

                int height = 0;

                var getLayoutManager = this.RecyclerView.GetLayoutManager();
                RelativeLayout RelativeContainerToCustome = holder.ItemView.FindViewById<RelativeLayout>(Resource.Id.RelativeContainer);

                height = (fragmentHeight / 4);

                if (h.RelativeContainer != null)
                {
                    RecyclerView.LayoutParams param = (RecyclerView.LayoutParams)h.RelativeContainer.LayoutParameters;
                    param.Height = height;
                    h.RelativeContainer.LayoutParameters = param;
                    h.RelativeContainer.Visibility = ViewStates.Visible;
                }
                if (h.CardContainer != null)
                {
                    ViewGroup.LayoutParams param = h.CardContainer.LayoutParameters;
                    param.Height = height;
                    h.CardContainer.LayoutParameters = param;
                    h.CardContainer.Visibility = ViewStates.Visible;
                }

                if (!string.IsNullOrEmpty(itemProduct.ItemType))
                {
                    if (h.DeleteItems != null)
                    {
                        FrameLayout DeleteItems = h.DeleteItems as FrameLayout;
                        if (DeleteItems != null)
                        {
                            DeleteItems.Visibility = ViewStates.Visible;
                            DeleteItems.Tag = itemProduct.ItemPositionForEdit;
                            DeleteItems.SetOnClickListener(this);
                        }
                    }
                }
            }

            if (!this.isModeEditNow)
            {
                int fragmentWidth = this.MainActivity.Window.DecorView.Width;
                //int cardMargin = dpToPx((int)5);
                int cardElevation = dpToPx((int)4);

                int width = 0;

                var getLayoutManager = this.RecyclerView.GetLayoutManager();
                RelativeLayout RelativeContainerToCustome = holder.ItemView.FindViewById<RelativeLayout>(Resource.Id.RelativeContainer);

                width = (fragmentWidth) / 3;

                if (h.RelativeContainer != null)
                {
                    RecyclerView.LayoutParams param = (RecyclerView.LayoutParams)h.RelativeContainer.LayoutParameters;
                    param.Width = width;
                    h.RelativeContainer.LayoutParameters = param;
                    h.RelativeContainer.Visibility = ViewStates.Visible;
                }
                if (h.CardContainer != null)
                {
                    ViewGroup.LayoutParams param = h.CardContainer.LayoutParameters;
                    param.Width = width;
                    h.CardContainer.LayoutParameters = param;
                    h.CardContainer.Visibility = ViewStates.Visible;
                }
            }
        }
        public void OnClick(View v)
        {
            switch(v.Id)
            {
                case Resource.Id.HomeScreenEmptyProductOnEdit:
                    this.AddItems((int)v.Tag);
                    break;
                case Resource.Id.DeleteItems:
                    this.DeleteItems((int)v.Tag);
                    break;
            }
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View v = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_layout, parent, false);
            MyViewHolder holder = new MyViewHolder(v);

            ViewGroup NonGroupItemSection = holder.ItemView.FindViewById(Resource.Id.NonGroupItem) as ViewGroup;
            TextView NonGroupItemName = holder.ItemView.FindViewById(Resource.Id.ItemName) as TextView;
            ViewGroup GroupItemSection = holder.ItemView.FindViewById(Resource.Id.GroupItem) as ViewGroup;
            TextView GroupItemItemName1 = holder.ItemView.FindViewById(Resource.Id.ItemNameGrouping1) as TextView;
            TextView GroupItemItemName2 = holder.ItemView.FindViewById(Resource.Id.ItemNameGrouping2) as TextView;
            TextView GroupItemItemName3 = holder.ItemView.FindViewById(Resource.Id.ItemNameGrouping3) as TextView;
            TextView GroupItemItemName4 = holder.ItemView.FindViewById(Resource.Id.ItemNameGrouping4) as TextView;
            FrameLayout Container = holder.ItemView.FindViewById(Resource.Id.Container) as FrameLayout;
            FrameLayout ItemContainer = holder.ItemView.FindViewById(Resource.Id.ItemContainer) as FrameLayout;
            FrameLayout DeleteItems = holder.ItemView.FindViewById(Resource.Id.DeleteItems) as FrameLayout;

            FrameLayout HomeScreenEmptyProduct = holder.ItemView.FindViewById(Resource.Id.HomeScreenEmptyProduct) as FrameLayout;
            LinearLayout HomeScreenEmptyProductNotOnEdit = holder.ItemView.FindViewById(Resource.Id.HomeScreenEmptyProductNotOnEdit) as LinearLayout;
            FrameLayout HomeScreenEmptyProductOnEdit = holder.ItemView.FindViewById(Resource.Id.HomeScreenEmptyProductOnEdit) as FrameLayout;
            RelativeLayout RelativeContainer = holder.ItemView.FindViewById(Resource.Id.RelativeContainer) as RelativeLayout;
            AndroidX.CardView.Widget.CardView CardContainer = holder.ItemView.FindViewById(Resource.Id.CardContainer) as AndroidX.CardView.Widget.CardView;
            //ViewGroup IndicatorGrouping = holder.ItemView.FindViewById(Resource.Id.IndicatorGrouping) as ViewGroup;

            if (NonGroupItemSection != null)
                holder.NonGroupItemSection = NonGroupItemSection;
            //holder.Views.Add("NonGroupItemSection", NonGroupItemSection);
            if (NonGroupItemName != null)
                holder.NonGroupItemName = NonGroupItemName;
            //holder.Views.Add("NonGroupItemName", NonGroupItemName);
            if (GroupItemSection != null)
                holder.GroupItemSection = GroupItemSection;
            //holder.Views.Add("GroupItemSection", GroupItemSection);
            if (GroupItemItemName1 != null)
                holder.GroupItemItemName1 = GroupItemItemName1;
            //holder.Views.Add("GroupItemItemName1", GroupItemItemName1);
            if (GroupItemItemName2 != null)
                holder.GroupItemItemName2 = GroupItemItemName2;
            //holder.Views.Add("GroupItemItemName2", GroupItemItemName2);
            if (GroupItemItemName3 != null)
                holder.GroupItemItemName3 = GroupItemItemName3;
            //holder.Views.Add("GroupItemItemName3", GroupItemItemName3);
            if (GroupItemItemName4 != null)
                holder.GroupItemItemName4 = GroupItemItemName4;
            if (Container != null)
                holder.Container = Container;
            if (ItemContainer != null)
                holder.ItemContainer = ItemContainer;
            if (DeleteItems != null)
                holder.DeleteItems = DeleteItems;
            if (HomeScreenEmptyProduct != null)
                holder.HomeScreenEmptyProduct = HomeScreenEmptyProduct;
            if (HomeScreenEmptyProductNotOnEdit != null)
                holder.HomeScreenEmptyProductNotOnEdit = HomeScreenEmptyProductNotOnEdit;
            if (HomeScreenEmptyProductOnEdit != null)
                holder.HomeScreenEmptyProductOnEdit = HomeScreenEmptyProductOnEdit;
            if (RelativeContainer != null)
                holder.RelativeContainer = RelativeContainer;
            if (CardContainer != null)
                holder.CardContainer = CardContainer;

            //if (holder != null)
            //{
            //    Context context = this.MainActivity;

            //    int fragmentHeight = this.MainActivity.Window.DecorView.Height;
            //    int fragmentWidth = this.MainActivity.Window.DecorView.Width;
            //    int cardMargin = dpToPx((int)5);
            //    int cardElevation = dpToPx((int)4);

            //    int width = 0;
            //    int height = 0;

            //    var getLayoutManager = this.RecyclerView.GetLayoutManager();
            //    RelativeLayout RelativeContainerToCustome = holder.ItemView.FindViewById<RelativeLayout>(Resource.Id.RelativeContainer);
            //    //var spaceBetweenRow = 0;

            //    //if ((this.RecyclerView.GetLayoutManager() as GridLayoutManager).Orientation == GridLayoutManager.Vertical)
            //    //{
            //    //    spaceBetweenRow = 3 * 5;
            //    //}
            //    //else
            //    //{
            //    //    spaceBetweenRow = 0;
            //    //}

            //    var BottomMargin = 0;
            //    var TopMargin = 0;

            //    if (RelativeContainerToCustome != null)
            //    {
            //        RecyclerView.LayoutParams param = (RecyclerView.LayoutParams)RelativeContainerToCustome.LayoutParameters;
            //        BottomMargin = param.BottomMargin;
            //        TopMargin = param.TopMargin;
            //    }

            //    width = (fragmentWidth) / 3;
            //    // width = (int)((fragmentWidth / 3) - (8 * 2) - cardElevation - 5);
            //    //height = (int)((fragmentHeight / 4) - (8 * 2) - (cardElevation * 9) - 15);
            //    height = ((fragmentHeight / 4) - (8 * 2) - (cardElevation * 9) - 15);

            //    //Intersoft.Crosslight.Android.v7.CardView container = holder.ContentView.FindViewById<Intersoft.Crosslight.Android.v7.CardView>(Resource.Id.container);
            //    AndroidX.CardView.Widget.CardView container = holder.ItemView.FindViewById<AndroidX.CardView.Widget.CardView>(Resource.Id.CardContainer);

            //    if (container != null)
            //    {
            //        ViewGroup.LayoutParams param = container.LayoutParameters;
            //        param.Width = width;
            //        param.Height = height;
            //        container.LayoutParameters = param;
            //    }

            //    if (RelativeContainerToCustome != null)
            //    {
            //        RecyclerView.LayoutParams param = (RecyclerView.LayoutParams)RelativeContainerToCustome.LayoutParameters;
            //        param.Width = width;
            //        param.Height = height;
            //        //param.SetMargins(0, 0, 0, 0);

            //        RelativeContainerToCustome.LayoutParameters = param;
            //    }

            //    //int height = (int)((fragmentHeight / 3) - 5);

            //}

            return holder;
        }
        public void SetMode(bool value)
        {
            this.isModeEditNow = value;
        }
        public int dpToPx(int dp)
        {
            //DisplayMetrics displayMetrics = getContext().getResources().getDisplayMetrics();
            DisplayMetrics displayMetrics = this.MainActivity.Resources.DisplayMetrics;
            //return Math.round(dp * (displayMetrics.xdpi / DisplayMetrics.DENSITY_DEFAULT));  
            return (int)Math.Round(dp * (displayMetrics.Xdpi / (float)DisplayMetricsDensity.Default));
        }
        public void OnRowMoved(int fromPosition, int toPosition)
        {
            List<HomeScreenMenuItem> itemFromPosition = this.data.Where(o => o.ItemPositionForEdit == fromPosition && o.IsDeleted == false).ToList();
            List<HomeScreenMenuItem> itemToPosition = this.data.Where(o => o.ItemPositionForEdit == toPosition && o.IsDeleted == false).ToList();


            //5 - 1
            if (fromPosition < toPosition)
            {
                for (int i = fromPosition; i < toPosition; i++)
                {
                    HomeScreenMenuItem getX = this.data.FirstOrDefault(o => o.ItemPositionForEdit == i && o.IsDeleted == false);
                    HomeScreenMenuItem getY = this.data.FirstOrDefault(o => o.ItemPositionForEdit == (i + 1) && o.IsDeleted == false);

                    var getXSavedData = this.LastSavedData.Where(o => o.ItemPositionForEdit == i && o.IsDeleted == false).ToList();
                    var getYSavedData = this.LastSavedData.Where(o => o.ItemPositionForEdit == (i + 1) && o.IsDeleted == false).ToList();

                    var getXPos = i;
                    if (getX != null)
                        getXPos = getX.ItemPositionForEdit;
                    var getYPos = getY.ItemPositionForEdit;

                    if (getX != null && getY != null)
                    {
                        List<HomeScreenMenuItem> savedXGroupItemToChange = null;
                        List<HomeScreenMenuItem> savedYGroupItemToChange = null;
                        if (!string.IsNullOrEmpty(getX.GroupName))
                        {
                            if (this.GroupDictionary.ContainsKey(getX.GroupingId))
                            {
                                List<HomeScreenMenuItem> findAllItemInGrouping = this.GroupDictionary[getX.GroupingId];

                                if (this.GroupDictionary.ContainsKey(getX.GroupingId))
                                {
                                    savedXGroupItemToChange = this.GroupDictionary[getX.GroupingId];
                                }

                                this.GroupDictionary.Remove(getX.GroupingId);
                            }
                        }
                        if (!string.IsNullOrEmpty(getY.GroupName))
                        {
                            if (this.GroupDictionary.ContainsKey(getY.GroupingId))
                            {
                                List<HomeScreenMenuItem> findAllItemInGroupingTo = this.GroupDictionary[getY.GroupingId];

                                if (this.GroupDictionary.ContainsKey(getY.GroupingId))
                                {
                                    savedYGroupItemToChange = this.GroupDictionary[getY.GroupingId];
                                    this.GroupDictionary.Remove(getY.GroupingId);
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(getX.GroupName))
                        {
                            if (savedXGroupItemToChange != null)
                            {
                                foreach (HomeScreenMenuItem itemInGrouping in savedXGroupItemToChange)
                                {
                                    var row = 4;
                                    var column = 3;
                                    //itemInGrouping.ItemPositionForEdit = getYPos;
                                    var CountItemPosition = ((getYPos / 12) * 12) + ((getYPos % column) * row) + ((getYPos % 12) / column);
                                    itemInGrouping.ItemPosition = CountItemPosition;
                                }

                                this.GroupDictionary.Add(getX.GroupingId, savedXGroupItemToChange);
                            }

                            foreach (var item in getXSavedData)
                            {
                                var row = 4;
                                var column = 3;
                                //item.ItemPositionForEdit = getYPos;
                                var CountItemPosition = ((getYPos / 12) * 12) + ((getYPos % column) * row) + ((getYPos % 12) / column);
                                item.ItemPosition = CountItemPosition;
                            }
                        }
                        else
                        {
                            var row = 4;
                            var column = 3;
                            ///getX.ItemPositionForEdit = getYPos;
                            var CountItemPosition = ((getYPos / 12) * 12) + ((getYPos % column) * row) + ((getYPos % 12) / column);
                            getX.ItemPosition = CountItemPosition;
                            foreach (var item in getXSavedData)
                            {
                                //item.ItemPositionForEdit = getYPos;
                                var CountItemPositionDua = ((getYPos / 12) * 12) + ((getYPos % column) * row) + ((getYPos % 12) / column);
                                item.ItemPosition = CountItemPositionDua;
                            }
                        }

                        //kalau nilai si getY itu merupakan item grouping
                        if (!string.IsNullOrEmpty(getY.GroupName))
                        {
                            if (savedYGroupItemToChange != null)
                            {
                                foreach (HomeScreenMenuItem itemInGroupingTo in savedYGroupItemToChange)
                                {
                                    var row = 4;
                                    var column = 3;
                                    //itemInGroupingTo.ItemPositionForEdit = getXPos;
                                    var CountItemPosition = ((getXPos / 12) * 12) + ((getXPos % column) * row) + ((getXPos % 12) / column);
                                    itemInGroupingTo.ItemPosition = CountItemPosition;
                                }

                                this.GroupDictionary.Add(getY.GroupingId, savedYGroupItemToChange);
                            }

                            foreach (var item in getYSavedData)
                            {
                                var row = 4;
                                var column = 3;
                                //item.ItemPositionForEdit = getXPos;
                                var CountItemPosition = ((getXPos / 12) * 12) + ((getXPos % column) * row) + ((getXPos % 12) / column);
                                item.ItemPosition = CountItemPosition;
                            }
                        }
                        else
                        {
                            var row = 4;
                            var column = 3;
                            //getY.ItemPositionForEdit = getXPos;
                            var CountItemPosition = ((getXPos / 12) * 12) + ((getXPos % column) * row) + ((getXPos % 12) / column);
                            getY.ItemPosition = CountItemPosition;
                            foreach (var item in getYSavedData)
                            {
                                //item.ItemPositionForEdit = getXPos;
                                var CountItemPositionDua = ((getXPos / 12) * 12) + ((getXPos % column) * row) + ((getXPos % 12) / column);
                                item.ItemPosition = CountItemPositionDua;
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = fromPosition; i > toPosition; i--)
                {
                    HomeScreenMenuItem getX = this.data.FirstOrDefault(o => o.ItemPositionForEdit == i && o.IsDeleted == false);
                    HomeScreenMenuItem getY = this.data.FirstOrDefault(o => o.ItemPositionForEdit == (i - 1) && o.IsDeleted == false);

                    var getXSavedData = this.LastSavedData.Where(o => o.ItemPositionForEdit == i && o.IsDeleted == false).ToList();
                    var getYSavedData = this.LastSavedData.Where(o => o.ItemPositionForEdit == (i - 1) && o.IsDeleted == false).ToList();

                    var getXPos = i;
                    if (getX != null)
                        getXPos = getX.ItemPositionForEdit;
                    var getYPos = getY.ItemPositionForEdit;

                    if (getX != null && getY != null)
                    {
                        List<HomeScreenMenuItem> savedXGroupItemToChange = null;
                        List<HomeScreenMenuItem> savedYGroupItemToChange = null;
                        if (!string.IsNullOrEmpty(getX.GroupName))
                        {
                            if (this.GroupDictionary.ContainsKey(getX.GroupingId))
                            {
                                List<HomeScreenMenuItem> findAllItemInGrouping = this.GroupDictionary[getX.GroupingId];

                                if (this.GroupDictionary.ContainsKey(getX.GroupingId))
                                {
                                    savedXGroupItemToChange = this.GroupDictionary[getX.GroupingId];
                                }

                                this.GroupDictionary.Remove(getX.GroupingId);
                            }
                        }
                        if (!string.IsNullOrEmpty(getY.GroupName))
                        {
                            if (this.GroupDictionary.ContainsKey(getY.GroupingId))
                            {
                                List<HomeScreenMenuItem> findAllItemInGroupingTo = this.GroupDictionary[getY.GroupingId];

                                if (this.GroupDictionary.ContainsKey(getY.GroupingId))
                                {
                                    savedYGroupItemToChange = this.GroupDictionary[getY.GroupingId];
                                    this.GroupDictionary.Remove(getY.GroupingId);
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(getX.GroupName))
                        {
                            if (savedXGroupItemToChange != null)
                            {
                                foreach (HomeScreenMenuItem itemInGrouping in savedXGroupItemToChange)
                                {
                                    var column = 3;
                                    var row = 4;
                                    //itemInGrouping.ItemPositionForEdit = getYPos;
                                    var CountItemPosition = ((getYPos / 12) * 12) + ((getYPos % column) * row) + ((getYPos % 12) / column);
                                    itemInGrouping.ItemPosition = CountItemPosition;
                                }

                                this.GroupDictionary.Add(getX.GroupingId, savedXGroupItemToChange);
                            }
                            
                            foreach (var item in getXSavedData)
                            {
                                var column = 3;
                                var row = 4;
                                //item.ItemPositionForEdit = getYPos;
                                var CountItemPosition = ((getYPos / 12) * 12) + ((getYPos % column) * row) + ((getYPos % 12) / column);
                                item.ItemPosition = CountItemPosition;
                            }
                        }
                        else
                        {
                            var column = 3;
                            var row = 4;
                            //getX.ItemPositionForEdit = getYPos;
                            var CountItemPosition = ((getYPos / 12) * 12) + ((getYPos % column) * row) + ((getYPos % 12) / column);
                            getX.ItemPosition = CountItemPosition;
                            foreach (var item in getXSavedData)
                            {
                                //item.ItemPositionForEdit = getYPos;
                                var CountItemPositionDua = ((getYPos / 12) * 12) + ((getYPos % column) * row) + ((getYPos % 12) / column);
                                item.ItemPosition = CountItemPositionDua;
                            }
                        }

                        //kalau nilai si getY itu merupakan item grouping
                        if (!string.IsNullOrEmpty(getY.GroupName))
                        {
                            if (savedYGroupItemToChange != null)
                            {
                                foreach (HomeScreenMenuItem itemInGroupingTo in savedYGroupItemToChange)
                                {
                                    var column = 3;
                                    var row = 4;
                                    //itemInGroupingTo.ItemPositionForEdit = getXPos;
                                    var CountItemPosition = ((getXPos / 12) * 12) + ((getXPos % column) * row) + ((getXPos % 12) / column);
                                    itemInGroupingTo.ItemPosition = CountItemPosition;
                                }

                                this.GroupDictionary.Add(getY.GroupingId, savedYGroupItemToChange);
                            }

                            foreach (var item in getYSavedData)
                            {
                                var column = 3;
                                var row = 4;
                                //item.ItemPositionForEdit = getXPos;
                                var CountItemPosition = ((getXPos / 12) * 12) + ((getXPos % column) * row) + ((getXPos % 12) / column);
                                item.ItemPosition = CountItemPosition;
                            }
                        }
                        else
                        {
                            var column = 3;
                            var row = 4;
                            //getY.ItemPositionForEdit = getXPos;
                            var CountItemPosition = ((getXPos / 12) * 12) + ((getXPos % column) * row) + ((getXPos % 12) / column);
                            getY.ItemPosition = CountItemPosition;
                            foreach (var item in getYSavedData)
                            {
                                //item.ItemPositionForEdit = getXPos;
                                var CountItemPositionDua = ((getXPos / 12) * 12) + ((getXPos % column) * row) + ((getXPos % 12) / column);
                                item.ItemPosition = CountItemPosition;
                            }
                        }
                    }
                }
            }
        }
        public void OnGrouping(int fromPosition, int toPosition)
        {
            //var getItemFrom = this.Items.FirstOrDefault(o => o.Position == From);
            //var getFirstItemTo = this.Items.FirstOrDefault(o => o.Position == To);
            //var getItemTo = this.Items.Where(o => o.Position == To);
            var getItemFrom = this.data.FirstOrDefault(o => o.ItemPositionForEdit == fromPosition);
            var getFirstItemTo = this.data.FirstOrDefault(o => o.ItemPositionForEdit == toPosition);
            var getItemTo = this.data.Where(o => o.ItemPositionForEdit == toPosition);
            string GroupName = "Grouping";

            if (getItemFrom != null && getFirstItemTo != null && getItemTo != null)
            {
                if (!string.IsNullOrEmpty(getItemFrom.ItemType) && string.IsNullOrEmpty(getItemFrom.GroupName))
                {
                    if (!string.IsNullOrEmpty(getFirstItemTo.ItemType))
                    {
                        Guid generateGuid = Guid.NewGuid();
                        //getItemFrom.ItemPositionForEdit = toPosition;
                        //var row = 4;
                        //var column = 3;
                        //(pageCount * 12) + ((posUser % column) * row) + ((Position % 12) / column);
                        //var calculateItemPosition = ((toPosition / 12) * 12) + ((toPosition % column) / row) + ((toPosition % 12) * column);
                        getItemFrom.ItemPosition = getFirstItemTo.ItemPosition;

                        if (getFirstItemTo.GroupingId != Guid.Empty)
                            generateGuid = getFirstItemTo.GroupingId;

                        getItemFrom.GroupingId = generateGuid;
                        if (getFirstItemTo.GroupName != "" || !string.IsNullOrEmpty(getFirstItemTo.GroupName))
                            getItemFrom.GroupName = getFirstItemTo.GroupName;
                        else
                            getItemFrom.GroupName = GroupName;

                        foreach (var itemTo in getItemTo)
                        {
                            if (string.IsNullOrEmpty(itemTo.GroupName))
                            {
                                itemTo.GroupName = GroupName;
                                itemTo.GroupingId = generateGuid;
                            }
                        }
                    }
                }
            }

            this.NotifyDataSetChanged();
        }
        public List<HomeScreenMenuItem> getDataAdapter()
        {
            return this.data;
        }
        public override int ItemCount
        {
            get { return data.Count(); }
        }
        /*
         * Our Viewholder class.
         * Will hold our textview.
         */
        internal class MyViewHolder : RecyclerView.ViewHolder
        {
            public ViewGroup NonGroupItemSection;
            public TextView NonGroupItemName;
            public ViewGroup GroupItemSection;
            public TextView GroupItemItemName1;
            public TextView GroupItemItemName2;
            public TextView GroupItemItemName3;
            public TextView GroupItemItemName4;
            public FrameLayout Container;
            public FrameLayout ItemContainer;
            public FrameLayout DeleteItems;
            public FrameLayout HomeScreenEmptyProduct;
            public LinearLayout HomeScreenEmptyProductNotOnEdit;
            public FrameLayout HomeScreenEmptyProductOnEdit;
            public RelativeLayout RelativeContainer;
            public AndroidX.CardView.Widget.CardView CardContainer;
            //public ViewGroup IndicatorGrouping;
            public object Item { get; set; }
            public Dictionary<string, View> Views { get; }
            public MyViewHolder(View itemView)
                : base(itemView)
            {
                NonGroupItemSection = itemView.FindViewById<ViewGroup>(Resource.Id.NonGroupItem);
                NonGroupItemName = itemView.FindViewById<TextView>(Resource.Id.ItemName);
                GroupItemSection = itemView.FindViewById<ViewGroup>(Resource.Id.GroupItem);
                GroupItemItemName1 = itemView.FindViewById<TextView>(Resource.Id.ItemNameGrouping1);
                GroupItemItemName2 = itemView.FindViewById<TextView>(Resource.Id.ItemNameGrouping2);
                GroupItemItemName3 = itemView.FindViewById<TextView>(Resource.Id.ItemNameGrouping3);
                GroupItemItemName4 = itemView.FindViewById<TextView>(Resource.Id.ItemNameGrouping4);
                Container = itemView.FindViewById<FrameLayout>(Resource.Id.Container);
                ItemContainer = itemView.FindViewById<FrameLayout>(Resource.Id.ItemContainer);
                DeleteItems = itemView.FindViewById<FrameLayout>(Resource.Id.DeleteItems);
                HomeScreenEmptyProduct = itemView.FindViewById<FrameLayout>(Resource.Id.HomeScreenEmptyProduct);
                HomeScreenEmptyProductNotOnEdit = itemView.FindViewById<LinearLayout>(Resource.Id.HomeScreenEmptyProductNotOnEdit);
                HomeScreenEmptyProductOnEdit = itemView.FindViewById<FrameLayout>(Resource.Id.HomeScreenEmptyProductOnEdit);
                RelativeContainer = itemView.FindViewById<RelativeLayout>(Resource.Id.RelativeContainer);
                CardContainer = itemView.FindViewById<AndroidX.CardView.Widget.CardView>(Resource.Id.CardContainer);

            }
        }
    }
}