using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using AndroidX.RecyclerView.Widget;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Java.Interop;

namespace SampleGrouping
{
    public class Location
    {
        public int locationX { get; set; }
        public int locationY { get; set; }
        public int LayoutPosition { get; set; }
    }
    public class HomeScreenMenuItem
    { 
        public Guid HomeScreenMenuId { get; set; }
        public Guid HomeScreenMenuItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemType { get; set; }
        public int ItemPosition { get; set; }
        public List<string> ListGroupItemName { get; set; }
        public string GroupName { get; set; }
        public bool IsDeleted { get; set; }
        public Guid GroupingId { get; set; }
        public int ItemPositionForEdit
        {
            get
            {
                var row = 4;
                var column = 3;
                var value = ((this.ItemPosition / 12) * 12) + ((this.ItemPosition % 12) / row) + ((this.ItemPosition % row) * column);
                //_itemPositionForEdit = value;
                return value;
            }
            //set
            //{
            //    if (_itemPositionForEdit != value)
            //        _itemPositionForEdit = value;
            //}
        }
        public bool IsIndicatorGroupingShow { get; set; }
    }
    public class HomeScreenMenu
    {
        public Guid homeScreenMenuId { get; set; }
        public int pageSize { get; set; }
        public List<HomeScreenMenuItem> homeScreenMenuItem { get; set; }
    }

    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private RecyclerView recyclerView;
        public MyAdapter mAdapter;
        HomeScreenMenu homeScreenMenu = new HomeScreenMenu();
        List<HomeScreenMenuItem> listData = new List<HomeScreenMenuItem>();
        public Android.Widget.LinearLayout PageIndicatorNotEdit;
        public Android.Widget.LinearLayout PageIndicatorOnEdit;
        public Android.Widget.LinearLayout PinContainer;
        List<Android.Widget.LinearLayout> DotDictionary { get; set; }
        public OnScrollListener OnScrollListeners { get; set; }
        private ItemTouchHelper _mItemTouchHelper;
        public Android.Widget.LinearLayout BtnPagePrevious;
        public Android.Widget.TextView PageNumberText;
        public Android.Widget.LinearLayout BtnPageNext;
        public Android.Widget.FrameLayout AddPageSection;
        public Android.Widget.ImageView ImageAddPage;
        public bool IsItemInRecyclerViewDragging;
        public bool IgnoreScrollStateChanged { get; set; }

        public RecyclerView.ViewHolder ViewHolderInDragging { get; set; }
        public OnTouchEvent TouchEvent { get; set; }
        public int FromPositionToUseToTriggerTreeObserver { get; set; }
        public int ToPositionToUseToTriggerTreeObserver { get; set; }
        public bool IgnoreSetIndicatorForFirstTime { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            Android.Widget.LinearLayout pageIndicatorNotEdit = FindViewById<Android.Widget.LinearLayout>(Resource.Id.PageIndicatorNotOnEdit);
            pageIndicatorNotEdit.Visibility = ViewStates.Visible;
            this.PageIndicatorNotEdit = pageIndicatorNotEdit;

            Android.Widget.LinearLayout pageIndicatorOnEdit = FindViewById<Android.Widget.LinearLayout>(Resource.Id.PageIndicatorOnEdit);
            this.PageIndicatorOnEdit = pageIndicatorOnEdit;

            Android.Widget.LinearLayout btnPagePrevious = FindViewById<Android.Widget.LinearLayout>(Resource.Id.BtnPagePrevious);
            btnPagePrevious.Tag = "previous";
            btnPagePrevious.Click += IndicatorOnEdit_Click;
            this.BtnPagePrevious = btnPagePrevious;

            Android.Widget.TextView pageNumberText = FindViewById<Android.Widget.TextView>(Resource.Id.PageNumberText);
            this.PageNumberText = pageNumberText;

            Android.Widget.LinearLayout btnPageNext = FindViewById<Android.Widget.LinearLayout>(Resource.Id.BtnPageNext);
            btnPageNext.Tag = "next";
            btnPageNext.Click += IndicatorOnEdit_Click;
            this.BtnPageNext = btnPageNext;

            Android.Widget.FrameLayout addPageSection = FindViewById<Android.Widget.FrameLayout>(Resource.Id.AddPageSection);
            this.AddPageSection = addPageSection;

            Android.Widget.ImageView imageAddPage = FindViewById<Android.Widget.ImageView>(Resource.Id.ImageAddPage);
            imageAddPage.Click += AddPage_Click;
            this.ImageAddPage = imageAddPage;

            Android.Widget.LinearLayout pinContainer = FindViewById<Android.Widget.LinearLayout>(Resource.Id.PinContainer);
            this.PinContainer = pinContainer;

            SetSupportActionBar(toolbar);

            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            recyclerView.SetLayoutManager(new GridLayoutManager(this, 4, GridLayoutManager.Horizontal, false));

            PopulateRecyclerView();
            this.HandlePageIndicator(1);
        }
        
        public void PopulateRecyclerView()
        {
            listData.Add(new HomeScreenMenuItem { HomeScreenMenuItemId = Guid.NewGuid(), ItemName = "item 1", ItemPosition = 0, GroupName = "", ItemType= "product", ListGroupItemName = { }, IsDeleted =false, GroupingId = Guid.Empty });
            listData.Add(new HomeScreenMenuItem { HomeScreenMenuItemId = Guid.NewGuid(), ItemName = "item 2", ItemPosition = 1, GroupName = "", ItemType = "product", ListGroupItemName = { }, IsDeleted = false, GroupingId = Guid.Empty });
            listData.Add(new HomeScreenMenuItem { HomeScreenMenuItemId = Guid.NewGuid(), ItemName = "item 3", ItemPosition = 2, GroupName = "", ItemType = "product", ListGroupItemName = { }, IsDeleted = false, GroupingId = Guid.Empty });
            listData.Add(new HomeScreenMenuItem { HomeScreenMenuItemId = Guid.NewGuid(), ItemName = "item 4", ItemPosition = 3, GroupName = "", ItemType = "product", ListGroupItemName = { }, IsDeleted = false, GroupingId = Guid.Empty });
            listData.Add(new HomeScreenMenuItem { HomeScreenMenuItemId = Guid.NewGuid(), ItemName = "item 5", ItemPosition = 4, GroupName = "", ItemType = "product", ListGroupItemName = { }, IsDeleted = false, GroupingId = Guid.Empty });
            listData.Add(new HomeScreenMenuItem { HomeScreenMenuItemId = Guid.NewGuid(), ItemName = "item 6", ItemPosition = 5, GroupName = "", ItemType = "product", ListGroupItemName = { }, IsDeleted = false, GroupingId = Guid.Empty });
            listData.Add(new HomeScreenMenuItem { HomeScreenMenuItemId = Guid.NewGuid(), ItemName = "item 7", ItemPosition = 6, GroupName = "", ItemType = "product", ListGroupItemName = { }, IsDeleted = false, GroupingId = Guid.Empty });
            listData.Add(new HomeScreenMenuItem { HomeScreenMenuItemId = Guid.NewGuid(), ItemName = "item 8", ItemPosition = 7, GroupName = "", ItemType = "product", ListGroupItemName = { }, IsDeleted = false, GroupingId = Guid.Empty });
            listData.Add(new HomeScreenMenuItem { HomeScreenMenuItemId = Guid.NewGuid(), ItemName = "item 9", ItemPosition = 8, GroupName = "", ItemType = "product", ListGroupItemName = { }, IsDeleted = false, GroupingId = Guid.Empty });
            listData.Add(new HomeScreenMenuItem { HomeScreenMenuItemId = Guid.NewGuid(), ItemName = "item 10", ItemPosition = 9, GroupName = "", ItemType = "product", ListGroupItemName = { }, IsDeleted = false, GroupingId = Guid.Empty });
            listData.Add(new HomeScreenMenuItem { HomeScreenMenuItemId = Guid.NewGuid(), ItemName = "item 11", ItemPosition = 10, GroupName = "", ItemType = "product", ListGroupItemName = { }, IsDeleted = false, GroupingId = Guid.Empty });
            listData.Add(new HomeScreenMenuItem { HomeScreenMenuItemId = Guid.NewGuid(), ItemName = "item 12", ItemPosition = 11, GroupName = "", ItemType = "product", ListGroupItemName = { }, IsDeleted = false, GroupingId = Guid.Empty });
            listData.Add(new HomeScreenMenuItem { HomeScreenMenuItemId = Guid.NewGuid(), ItemName = "item 13", ItemPosition = 12, GroupName = "", ItemType = "product", ListGroupItemName = { }, IsDeleted = false, GroupingId = Guid.Empty });


            homeScreenMenu.homeScreenMenuId = Guid.NewGuid();
            homeScreenMenu.pageSize = 2;
            homeScreenMenu.homeScreenMenuItem = new List<HomeScreenMenuItem>();
            foreach (var item in listData)
            {
                item.HomeScreenMenuId = homeScreenMenu.homeScreenMenuId;
                homeScreenMenu.homeScreenMenuItem.Add(item);
            }

            mAdapter = new MyAdapter(listData, this, homeScreenMenu, recyclerView);

            this.OnScrollListeners = new OnScrollListener(this.recyclerView.GetLayoutManager(), this);
            this.recyclerView.AddOnScrollListener(this.OnScrollListeners);

            this.SetLayoutTypeCore(true);
            recyclerView.SetAdapter(mAdapter);
        }

        private void SetLayoutTypeCore(bool refreshAdapter)
        {
            if (recyclerView != null)
            {
                if (recyclerView != null)
                {
                    CustomeView.SnapToBlock snapToBlock = new CustomeView.SnapToBlock(12, this);
                    snapToBlock.AttachToRecyclerView(recyclerView);
                }
            }
        }

        public IMenu DataMenu { get; set; }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            this.DataMenu = menu;
            return true;
        }
        public void ShowHideDeletePage(bool IsDeletePageShow)
        {
            var findAddItemMenu = this.DataMenu.FindItem(Resource.Id.delete_page);
            findAddItemMenu.SetVisible(IsDeletePageShow);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.edit_menu)
            {
                if (this.mAdapter.ViewHolderLocationDictionary.Count > 0)
                {
                    this.mAdapter.ViewHolderLocationDictionary.Clear();
                }

                var checkPositionPage = this.FindPagePosition(this.recyclerView.GetLayoutManager());
                this.mAdapter.isModeChange = true;
                this.PageIndicatorOnEdit.Visibility = ViewStates.Visible;
                this.PageIndicatorNotEdit.Visibility = ViewStates.Gone;
                this.PageNumberText.Text = checkPositionPage.ToString();

                this.HandlePageIndicator(checkPositionPage);

                var checkPageSize = (this.mAdapter.ItemCount / 12);
                if (checkPageSize > 1)
                {
                    var findAddItemMenu = this.DataMenu.FindItem(Resource.Id.delete_page);
                    findAddItemMenu.SetVisible(true);
                }
                else
                {
                    var findAddItemMenu = this.DataMenu.FindItem(Resource.Id.delete_page);
                    findAddItemMenu.SetVisible(false);
                }

                OnTouchEvent touch = new OnTouchEvent(recyclerView, this, mAdapter);
                this.TouchEvent = touch;
                recyclerView.SetOnTouchListener(touch);

                ItemTouchHelper.Callback callback = new ItemMoveCallback(mAdapter, recyclerView, this, mAdapter);
                this._mItemTouchHelper = new ItemTouchHelper(callback);
                this._mItemTouchHelper.AttachToRecyclerView(recyclerView);

                mAdapter.SetMode(true);

                var pagePosition = this.FindPagePosition(this.recyclerView.GetLayoutManager());
                mAdapter.LoadDataAfterChangeMode((pagePosition));

                recyclerView.SetLayoutManager(new GridLayoutManager(this, 3, GridLayoutManager.Vertical, false));
                recyclerView.SetAdapter(mAdapter);

                mAdapter.NotifyDataSetChanged();
                return true;
            }
            if (id == Resource.Id.save_menu)
            {
                if (this.mAdapter.ViewHolderLocationDictionary.Count > 0)
                {
                    this.mAdapter.ViewHolderLocationDictionary.Clear();
                }

                recyclerView.SetOnTouchListener(null);
                this.mAdapter.isModeChange = true;
                this.recyclerView.AddOnScrollListener(this.OnScrollListeners);
                this.recyclerView.NestedScrollingEnabled = true;

                var checkPositionPage = this.FindPagePosition(this.recyclerView.GetLayoutManager());
                this.HandlePageIndicator(checkPositionPage);

                this.PageIndicatorOnEdit.Visibility = ViewStates.Gone;
                this.PageIndicatorNotEdit.Visibility = ViewStates.Visible;
                var findAddItemMenu = this.DataMenu.FindItem(Resource.Id.delete_page);
                findAddItemMenu.SetVisible(false);

                if (this._mItemTouchHelper != null)
                    this._mItemTouchHelper.AttachToRecyclerView(null);

                mAdapter.SetMode(false);

                recyclerView.SetLayoutManager(new GridLayoutManager(this, 4, GridLayoutManager.Horizontal, false));

                recyclerView.SetAdapter(mAdapter);

                mAdapter.LoadData(mAdapter.LastSavedData, mAdapter.HomeScreenMenu);
                mAdapter.NotifyDataSetChanged();

                this.IgnoreScrollStateChanged = true;

                var updateIndicator = this.HandlePageIndicator(mAdapter.LastPagePositionBeforeInEditMode);

                if (updateIndicator)
                    this.recyclerView.SmoothScrollToPosition((mAdapter.LastPagePositionBeforeInEditMode * 12) - 1);

                Task.Run(async () => 
                {
                    await Task.Delay(3000);
                    this.IgnoreScrollStateChanged = false;
                });

                return true;
            }
            if (id == Resource.Id.delete_page)
            {
                HomeScreenMenu homeScreenMenuUpdate = mAdapter.DeletePage(this.homeScreenMenu);
                this.homeScreenMenu = homeScreenMenuUpdate;
                this.PageNumberText.Text = mAdapter.LastPagePositionBeforeInEditMode.ToString();
                mAdapter.NotifyDataSetChanged();
            }

            return base.OnOptionsItemSelected(item);
        }

        public void ShowUndo()
        {
            Snackbar snackBar = Snackbar.Make(this.recyclerView, mAdapter.GetUndoTitle(), Snackbar.LengthLong);
            snackBar.SetAction("Undo Action", new ClickListener(this.DummyFunction, ""));
            snackBar.Show();
        }
        public void DummyFunction(object parameter)
        {
            if (mAdapter.LastActionDoing == "DeleteItem")
                mAdapter.UndoDeletedItem();
            if (mAdapter.LastActionDoing == "DeletePage")
            {
                HomeScreenMenu getUpdatedHomeScreenMenu= mAdapter.UndoDeletePage(this.homeScreenMenu);
                this.homeScreenMenu = getUpdatedHomeScreenMenu;
                if (mAdapter.isPageMovedAfterDeletePage)
                    this.PageNumberText.Text = mAdapter.LastPagePositionBeforeInEditMode.ToString();
                mAdapter.NotifyDataSetChanged();
            }
        }
        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (View.IOnClickListener)null).Show();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void Dispose(bool disposing)
        {
            if (this.recyclerView != null)
                this.recyclerView.RemoveOnScrollListener(this.OnScrollListeners);
            if (this.ImageAddPage != null)
                this.ImageAddPage.Click -= AddPage_Click;
            if (this.mAdapter.ViewHolderLocationDictionary != null)
            {
                this.mAdapter.ViewHolderLocationDictionary = null;

            }

            base.Dispose(disposing);
        }

        public int FindPagePosition(AndroidX.RecyclerView.Widget.RecyclerView.LayoutManager layoutManager)
        {
            GridLayoutManager gridLayoutManager = layoutManager as GridLayoutManager;
            var z = gridLayoutManager.FindLastCompletelyVisibleItemPosition();
            double convertZ = (Convert.ToDouble(z));
            double hasilBagi = (convertZ + 1) / 12;
            int ceilingHasil = (Convert.ToInt32(Math.Ceiling(hasilBagi)));
            return ceilingHasil;
        }

        public bool HandlePageIndicator(int PageActive)
        {
            this.PinContainer.RemoveAllViewsInLayout();
            double itemCount = (Convert.ToDouble(mAdapter.data.Count) / 12);
            //if (this.mAdapter.isModeEditNow)
            //    itemCount = (Convert.ToDouble(mAdapter.LastSavedData.Count) / 12);
            double pageIndicatorValue = Math.Ceiling(itemCount);

            for (int i = 1; i <= pageIndicatorValue; i++)
            {
                Android.Widget.LinearLayout linearLayout = new Android.Widget.LinearLayout(this);
                if (i == PageActive)
                {
                    linearLayout.LayoutParameters = new Android.Widget.LinearLayout.LayoutParams(16, 16) { LeftMargin = 8, RightMargin = 8 };
                    linearLayout.SetBackgroundResource(Resource.Drawable.circular_green);
                }
                else
                {
                    linearLayout.LayoutParameters = new Android.Widget.LinearLayout.LayoutParams(16, 16) { LeftMargin = 8, RightMargin = 8 };
                    linearLayout.SetBackgroundResource(Resource.Drawable.rounded_accent);
                }
                this.PinContainer.AddView(linearLayout);
            }
            this.PinContainer.RequestLayout();
            this.PinContainer.Invalidate();

            return true;
        }
        public void IndicatorOnEdit_Click(object sender, EventArgs e)
        {
            Android.Widget.LinearLayout view = sender as Android.Widget.LinearLayout;
            string btnClickMode = (string)view.Tag;
            mAdapter.IndicatorOnEditModeClick(btnClickMode);
            mAdapter.NotifyDataSetChanged();
            this.PageNumberText.Text = mAdapter.LastPagePositionBeforeInEditMode.ToString();
        }
        public void AddPage_Click(object sender, EventArgs e)
        {
            HomeScreenMenu homeScreenMenuUpdate = mAdapter.AddPage(this.homeScreenMenu);
            this.homeScreenMenu = homeScreenMenuUpdate;
            this.PageNumberText.Text = mAdapter.LastPagePositionBeforeInEditMode.ToString();
            mAdapter.NotifyDataSetChanged();
        }
    }
    internal class ClickListener : Java.Lang.Object, View.IOnClickListener
    {
        #region Constructors

        internal ClickListener(Action<object> callback, object parameter)
        {
            this.CallBack = callback;
            this.Parameter = parameter;
        }

        #endregion

        #region Properties

        private Action<object> CallBack { get; set; }

        private object Parameter { get; set; }

        #endregion

        #region IOnClickListener Members

        /// <summary>
        /// Called when presenter is clicked
        /// </summary>
        /// <param name="v">The presenter view.</param>
        public void OnClick(View v)
        {
            this.CallBack(this.Parameter);
        }

        #endregion
    }
    public class OnScrollListener : RecyclerView.OnScrollListener
    {
        #region Properties
        AndroidX.RecyclerView.Widget.RecyclerView.LayoutManager layoutManager;
        public MainActivity MainActivity { get; set; }
        #endregion
        public OnScrollListener(AndroidX.RecyclerView.Widget.RecyclerView.LayoutManager layoutManager, MainActivity mainActivity)
        {
            this.layoutManager = layoutManager;
            this.MainActivity = mainActivity;
        }
        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            base.OnScrolled(recyclerView, dx, dy);
        }
        public View FindOneVisibleChild(int fromIndex, int toIndex, bool completelyVisible, RecyclerView recyclerView, GridLayoutManager gridLayoutManager)
        {
            var helper = OrientationHelper.CreateOrientationHelper(recyclerView.GetLayoutManager(), gridLayoutManager.Orientation);
            int start = helper.StartAfterPadding;
            int end = helper.EndAfterPadding;

            int next = toIndex > fromIndex ? 1 : -1;

            for (int i = fromIndex; i != toIndex; i+=next)
            {
                View child = recyclerView.GetChildAt(i);
                int childStart = gridLayoutManager.GetDecoratedLeft(child);
                int childEnd = gridLayoutManager.GetDecoratedRight(child);

                if (childStart < end && childEnd > start)
                {
                    if (completelyVisible)
                    {
                        if (childStart >= start && childEnd <= end)
                        {
                            return child;
                        }
                    }
                    else
                    {
                        return child;
                    }
                }
            }
            return null;
        }
        public override void OnScrollStateChanged(RecyclerView recyclerView, int newState)
        {
            base.OnScrollStateChanged(recyclerView, newState);
            if (newState == RecyclerView.ScrollStateIdle || newState == RecyclerView.ScrollStateSettling)
            {
                if (!this.MainActivity.IgnoreScrollStateChanged)
                {
                    GridLayoutManager gridLayoutManager = this.layoutManager as GridLayoutManager;
                    var checkData = gridLayoutManager.ChildCount;
                    var checkChildCountInRecyclerView = recyclerView.ChildCount;

                    
                    var findLastCompletelyVisibleItemPosition = this.FindOneVisibleChild((checkChildCountInRecyclerView - 1), -1, true, recyclerView, gridLayoutManager);

                    int child = 0;
                    if (findLastCompletelyVisibleItemPosition != null)
                    {
                        child = recyclerView.GetChildLayoutPosition(findLastCompletelyVisibleItemPosition);
                    }

                    var z = child;

                    if (this.MainActivity.mAdapter.isModeEditNow)
                    {
                        var checkPage = 1;
                        if (this.MainActivity.mAdapter.LastPagePositionBeforeInEditMode > 1)
                            checkPage = this.MainActivity.mAdapter.LastPagePositionBeforeInEditMode;

                        z = ((12 * (checkPage - 1)) + child);
                    }

                    double convertZ = (Convert.ToDouble(z));
                    double hasilBagi = (convertZ + 1) / 12;
                    int ceilingHasil = (Convert.ToInt32(Math.Ceiling(hasilBagi)));
                    this.MainActivity.HandlePageIndicator(ceilingHasil);

                    if (this.MainActivity.PageNumberText != null)
                        this.MainActivity.PageNumberText.Text = ceilingHasil.ToString();
                }
            }
        }
    }
    public class ItemMoveCallback : ItemTouchHelper.Callback
    {
        private ItemTouchHelperContract mAdapter;
        public int dataFrom;
        public int dataTo;
        public bool ActionDragTodo;
        private RecyclerView.Adapter Adapter
        {
            get
            {
                if (this.RecyclerView != null)
                    return this.RecyclerView.GetAdapter();

                return null;
            }
        }
        private RecyclerView RecyclerView { get; set; }
        private MainActivity MainActivity { get; set; }
        Android.Widget.LinearLayout GroupingIndicator { get; set; }
        Android.Widget.LinearLayout Container { get; set; }
        public MyAdapter MyAdapter { get; set; }
        public ItemMoveCallback(ItemTouchHelperContract adapter, RecyclerView recyclerView, MainActivity MainActivity, MyAdapter myAdapter)
        {
            mAdapter = adapter;
            this.RecyclerView = recyclerView;
            this.MainActivity = MainActivity;
            this.MyAdapter = myAdapter;

        }

        public override bool IsLongPressDragEnabled => base.IsLongPressDragEnabled;
        public override bool IsItemViewSwipeEnabled => base.IsItemViewSwipeEnabled;


        public override int GetMovementFlags(RecyclerView p0, RecyclerView.ViewHolder p1)
        {
            var data = this.MyAdapter.data.FirstOrDefault(o => o.ItemPositionForEdit == p1.LayoutPosition);

            if (this.MyAdapter.isModeEditNow)
            {
                var checkPage = 1;
                if (this.MyAdapter.LastPagePositionBeforeInEditMode > 1)
                    checkPage = this.MyAdapter.LastPagePositionBeforeInEditMode;

                var findLayoutPosition = ((12 * (checkPage - 1)) + p1.LayoutPosition);

                data = this.MyAdapter.data.FirstOrDefault(o => o.ItemPositionForEdit == findLayoutPosition);
            }

            if (!string.IsNullOrEmpty(data.ItemType))
            {
                int dragFlags = ItemTouchHelper.Up | ItemTouchHelper.Down | ItemTouchHelper.Start | ItemTouchHelper.End;
                return MakeMovementFlags(dragFlags, 0);
            }
            else
            {
                int dragFlags = ItemTouchHelper.ActionStateIdle;
                return MakeMovementFlags(dragFlags, 0);
            }
            
            
        }
        public override void OnMoved(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, int fromPos, RecyclerView.ViewHolder target, int toPos, int x, int y)
        {
            base.OnMoved(recyclerView, viewHolder, fromPos, target, toPos, x, y);
            
        }
        public override bool OnMove(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder,RecyclerView.ViewHolder target)
        {
            int from = viewHolder.LayoutPosition;
            int to = target.LayoutPosition;

            int from2 = viewHolder.AdapterPosition;
            int to2 = target.AdapterPosition;

            this.Target = target;
            this.ToPos = viewHolder;



            this.dataFrom = from;
            this.dataTo = to;

            int[] screen = new int[2];

            viewHolder.ItemView.GetLocationOnScreen(screen);

            int checkViewHolderX = screen[0];
            int checkViewHolderY = screen[1];

            target.ItemView.GetLocationOnScreen(screen);

            int checkTargetX = screen[0];
            int checkTargetY = screen[1];

            int diffX = Math.Abs(checkViewHolderX - checkTargetX);
            int diffY = Math.Abs(checkViewHolderY - checkTargetY);


            if (diffX > 100 || diffY > 100)
            {
                var checkPage = 1;
                if (this.MyAdapter.LastPagePositionBeforeInEditMode > 1)
                    checkPage = this.MyAdapter.LastPagePositionBeforeInEditMode;

                var findDataFromLayoutPositionInData = ((12 * (checkPage - 1)) + this.dataFrom);
                var findDataToLayoutPositionInData = ((12 * (checkPage - 1)) + this.dataTo);

                mAdapter.OnRowMoved(findDataFromLayoutPositionInData, findDataToLayoutPositionInData);
                this.MyAdapter.NotifyItemMoved(this.dataFrom, this.dataTo);

                this.MainActivity.FromPositionToUseToTriggerTreeObserver = this.dataFrom;
                this.MainActivity.ToPositionToUseToTriggerTreeObserver = this.dataTo;

                this.MainActivity.IgnoreSetIndicatorForFirstTime = true;

                Task.Run(async () =>
                {
                    await Task.Delay(2000);
                    this.MainActivity.IgnoreSetIndicatorForFirstTime = false;
                });
            }

            return true;
        }
        public RecyclerView.ViewHolder Target { get; set; }
        public RecyclerView.ViewHolder ToPos { get; set; }
        public int DiffX { get; set; }
        public int DiffY { get; set; }
        public bool IsStillDrag { get; set; }
        public bool OnMoveCalled { get; set; }
        public RecyclerView.ViewHolder ViewHolder { get; set; }
        
        public override void OnSwiped(RecyclerView.ViewHolder p0, int p1)
        {
            throw new NotImplementedException();
        }
        public override void OnSelectedChanged(RecyclerView.ViewHolder viewHolder, int actionState)
        {
            base.OnSelectedChanged(viewHolder, actionState);
            switch (actionState)
            {
                case ItemTouchHelper.ActionStateDrag:
                    this.ViewHolder = viewHolder;
                    this.MainActivity.ViewHolderInDragging = viewHolder;
                    this.MainActivity.IsItemInRecyclerViewDragging = true;
                    break;
                case ItemTouchHelper.ActionStateIdle:

                    int[] screen = new int[2];
                    this.ViewHolder.ItemView.GetLocationOnScreen(screen);

                    var checkDataHolder = this.MyAdapter.data.FirstOrDefault(o => o.ItemPositionForEdit == this.ViewHolder.LayoutPosition);

                    if (this.MyAdapter.isModeEditNow)
                    {
                        var customizePosition = ((12 * (this.MyAdapter.LastPagePositionBeforeInEditMode - 1)) + this.ViewHolder.LayoutPosition);
                        checkDataHolder = this.MyAdapter.data.FirstOrDefault(o => o.ItemPositionForEdit == customizePosition && o.IsDeleted == false);
                    }

                    if (string.IsNullOrEmpty(checkDataHolder.GroupName) || checkDataHolder.GroupingId == Guid.Empty)
                    {
                        int viewHolderLocationX = screen[0];
                        int viewHolderLocationY = screen[1];

                        Location viewHolderLocation = new Location();
                        viewHolderLocation.locationX = viewHolderLocationX;
                        viewHolderLocation.locationY = viewHolderLocationY;

                        var findLayoutPositionInDictionary = this.MyAdapter.ViewHolderLocationDictionary.Where(o => o.Key.LayoutPosition == this.ViewHolder.LayoutPosition).ToList();

                        foreach (KeyValuePair<RecyclerView.ViewHolder, Location> dataLocation in this.MyAdapter.ViewHolderLocationDictionary)
                        {
                            int locationX = dataLocation.Value.locationX;
                            int locationY = dataLocation.Value.locationY;

                            int diffX = Math.Abs(viewHolderLocation.locationX - locationX);
                            int diffY = Math.Abs(viewHolderLocation.locationY - locationY);

                            if (dataLocation.Key.LayoutPosition != this.ViewHolder.LayoutPosition)
                            {
                                var checkPage = 1;
                                if (this.MyAdapter.LastPagePositionBeforeInEditMode > 1)
                                    checkPage = this.MyAdapter.LastPagePositionBeforeInEditMode;

                                var findFromPositionLayoutPositionInData = ((12 * (checkPage - 1)) + this.ViewHolder.LayoutPosition);
                                var findToPositionLayoutPositionInData = ((12 * (checkPage - 1) + dataLocation.Key.LayoutPosition));

                                if (diffX > 20 && diffY > 20 && diffX < 100 && diffY < 100)
                                {
                                    mAdapter.OnGrouping(findFromPositionLayoutPositionInData, findToPositionLayoutPositionInData);
                                    this.MyAdapter.NotifyItemRangeChanged(0, this.MyAdapter.ItemCount);

                                    var getDataInDataLocation = this.MyAdapter.data.Where(o => o.ItemPositionForEdit == findToPositionLayoutPositionInData);

                                    foreach (var item in getDataInDataLocation)
                                        item.IsIndicatorGroupingShow = false;

                                }
                            }
                        }

                        mAdapter.LoadDataAfterGrouping();
                    }
                    this.MainActivity.IsItemInRecyclerViewDragging = false;
                    //this.dataFrom = -1;
                    break;
            }
        }
        public interface ItemTouchHelperContract
        {
            public void OnRowMoved(int fromPosition, int toPosition);
            public void OnGrouping(int fromPosition, int toPosition);
            public bool LoadDataAfterGrouping();
            public void SetDataToShowGroupingIndicator(int TargetPosition, bool IsTargetIndicatorGroupingShowed);
            public List<HomeScreenMenuItem> getDataAdapter();
        }
    }

    public class OnTouchEvent : Java.Lang.Object, View.IOnTouchListener
    {
        private RecyclerView RecyclerView { get; set; }
        private MainActivity MainActivity { get; set; }
        public MyAdapter MyAdapter { get; set; }
        private RecyclerView.Adapter Adapter
        {
            get
            {
                if (this.RecyclerView != null)
                    return this.RecyclerView.GetAdapter();

                return null;
            }
        }
        public RecyclerView.ViewHolder ViewHolder { get; set; }
        public OnTouchEvent(RecyclerView recyclerView, MainActivity MainActivity, MyAdapter myAdapter)
        {
            this.RecyclerView = recyclerView;
            this.MainActivity = MainActivity;
            this.MyAdapter = myAdapter;
        }
        public bool OnTouch(View v, MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Move:
                    
                    if (this.MainActivity.IsItemInRecyclerViewDragging)
                    {
                        
                        RecyclerView.ViewHolder holder = this.MainActivity.ViewHolderInDragging;
                        if (holder == null)
                            return false;

                        this.ViewHolder = holder;

                        var checkDataHolder = this.MyAdapter.data.FirstOrDefault(o => o.ItemPositionForEdit == this.ViewHolder.LayoutPosition);

                        if (this.MyAdapter.isModeEditNow)
                        {
                            var customizePosition = ((12 * (this.MyAdapter.LastPagePositionBeforeInEditMode - 1)) + this.ViewHolder.LayoutPosition);
                            checkDataHolder = this.MyAdapter.data.FirstOrDefault(o => o.ItemPositionForEdit == customizePosition && o.IsDeleted == false);
                        }

                        if (string.IsNullOrEmpty(checkDataHolder.GroupName) && checkDataHolder.GroupingId == Guid.Empty)
                        {
                            int[] screenLoc = new int[2];

                            this.ViewHolder.ItemView.GetLocationOnScreen(screenLoc);

                            int viewHolderLocationXGet = screenLoc[0];
                            int viewHolderLocationYGet = screenLoc[1];

                            Location viewHolderLocationToCheck = new Location();
                            viewHolderLocationToCheck.locationX = viewHolderLocationXGet;
                            viewHolderLocationToCheck.locationY = viewHolderLocationYGet;

                            //var findTest = this.MyAdapter.ViewHolderLocationDictionary.Where(o => o.Key.LayoutPosition == this.ViewHolder.LayoutPosition).ToList();
                            
                            var findTest = this.MyAdapter.ViewHolderLocationDictionary.Where(o => o.Key.LayoutPosition == this.ViewHolder.LayoutPosition).ToList();

                            bool isItemMove = false;
                            // untuk check apa kah item yg di klik sudah bergerak apa blm
                            foreach (var item in findTest)
                            {
                                int[] screenItemLoc = new int[2];

                                int checkX = Math.Abs(viewHolderLocationToCheck.locationX - item.Value.locationX);
                                int checkY = Math.Abs(viewHolderLocationToCheck.locationY - item.Value.locationY);

                                if (checkX > 10 && checkY > 10)
                                    isItemMove = true;
                            }

                            foreach (KeyValuePair<RecyclerView.ViewHolder, Location> dataLocation in this.MyAdapter.ViewHolderLocationDictionary)
                            {
                                int locationX = dataLocation.Value.locationX;
                                int locationY = dataLocation.Value.locationY;

                                System.Diagnostics.Debug.Write("Position :" + dataLocation.Key.LayoutPosition + " Location X :" + locationX + " Location Y :" + locationY);

                                int persentageX = Math.Abs(viewHolderLocationToCheck.locationX - locationX);
                                int persentageY = Math.Abs(viewHolderLocationToCheck.locationY - locationY);

                                if (dataLocation.Key.GetHashCode() != this.ViewHolder.GetHashCode())
                                {
                                    var checkPage = 1;
                                    if (this.MyAdapter.LastPagePositionBeforeInEditMode > 1)
                                        checkPage = this.MyAdapter.LastPagePositionBeforeInEditMode;

                                    var findLayoutPositionForDataLocation = ((12 * (checkPage - 1)) + dataLocation.Key.LayoutPosition);

                                    if (persentageX > 100 || persentageY > 100)
                                    {
                                        List<HomeScreenMenuItem> itemToChangeSizeBack = this.MyAdapter.data.Where(o => o.ItemPositionForEdit == findLayoutPositionForDataLocation).ToList();
                                        int updatePrevShowingIndicator = -1;
                                        foreach (var item in itemToChangeSizeBack)
                                        {
                                            if (item.IsIndicatorGroupingShow)
                                                updatePrevShowingIndicator = item.ItemPositionForEdit;
                                            item.IsIndicatorGroupingShow = false;
                                        }
                                        if (updatePrevShowingIndicator > -1)
                                        {
                                            this.MyAdapter.NotifyItemChanged(dataLocation.Key.LayoutPosition);
                                        }
                                    }
                                    else if (persentageX > 20 && persentageY > 20 && persentageX < 100 && persentageY < 100)
                                    {
                                        List<HomeScreenMenuItem> itemToChangeSize = this.MyAdapter.data.Where(o => o.ItemPositionForEdit == findLayoutPositionForDataLocation).ToList();
                                        foreach (var item in itemToChangeSize)
                                        {
                                            if (checkDataHolder.GroupingId == Guid.Empty || string.IsNullOrEmpty(checkDataHolder.GroupName))
                                            {
                                                if (!this.MainActivity.IgnoreSetIndicatorForFirstTime)
                                                    item.IsIndicatorGroupingShow = true;
                                            }
                                        }
                                        this.MyAdapter.NotifyItemChanged(dataLocation.Key.LayoutPosition);
                                    }
                                }
                            }
                        }
                    }
                    break;
            }
            return false;
        }
    }
}
