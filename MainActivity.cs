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

namespace SampleGrouping
{
    public class HomeScreenMenuItem
    { 
        public Guid HomeScreenMenuId { get; set; }
        public Guid HomeScreenMenuItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemType { get; set; }
        public int ItemPosition { get; set; }
        //public int UserItemPosition
        //{
        //    //int posUser = (pageCount * 12) + ((Position % 12) / row) + ((Position % row) * column);
        //    get { return }
        //}
        public List<string> ListGroupItemName { get; set; }
        public string GroupName { get; set; }
        public bool IsDeleted { get; set; }
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
        private MyAdapter mAdapter;
        HomeScreenMenu homeScreenMenu = new HomeScreenMenu();
        List<HomeScreenMenuItem> listData = new List<HomeScreenMenuItem>();
        public Android.Widget.LinearLayout PageIndicatorNotEdit;
        public Android.Widget.LinearLayout PageIndicatorOnEdit;
        public Android.Widget.LinearLayout PinContainer;
        List<Android.Widget.LinearLayout> DotDictionary { get; set; }
        public OnScrollListener OnScrollListeners { get; set; }
        private ItemTouchHelper _mItemTouchHelper;
        public Android.Widget.TextView PageNumberText;
        public Android.Widget.FrameLayout AddPageSection;
        public Android.Widget.ImageView ImageAddPage;

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
            if (this.mAdapter != null || this.mAdapter == null)
                this.mAdapter = new MyAdapter(this.listData, this, this.homeScreenMenu, this.recyclerView);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            //Android.Widget.LinearLayout groupingIndicator = FindViewById<Android.Widget.LinearLayout>(Resource.Id.IndicatorGrouping);
            //this.GroupingIndicator = groupingIndicator;

            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            Android.Widget.LinearLayout pageIndicatorNotEdit = FindViewById<Android.Widget.LinearLayout>(Resource.Id.PageIndicatorNotOnEdit);
            pageIndicatorNotEdit.Visibility = ViewStates.Visible;
            this.PageIndicatorNotEdit = pageIndicatorNotEdit;

            Android.Widget.LinearLayout pageIndicatorOnEdit = FindViewById<Android.Widget.LinearLayout>(Resource.Id.PageIndicatorOnEdit);
            this.PageIndicatorOnEdit = pageIndicatorOnEdit;

            Android.Widget.TextView pageNumberText = FindViewById<Android.Widget.TextView>(Resource.Id.PageNumberText);
            this.PageNumberText = pageNumberText;

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
            listData.Add(new HomeScreenMenuItem { HomeScreenMenuItemId = Guid.NewGuid(), ItemName = "item 1", ItemPosition = 0, GroupName = "", ItemType= "product", ListGroupItemName = { }, IsDeleted =false });
            listData.Add(new HomeScreenMenuItem { HomeScreenMenuItemId = Guid.NewGuid(), ItemName = "item 2", ItemPosition = 1, GroupName = "", ItemType = "product", ListGroupItemName = { }, IsDeleted = false });
            listData.Add(new HomeScreenMenuItem { HomeScreenMenuItemId = Guid.NewGuid(), ItemName = "item 3", ItemPosition = 2, GroupName = "", ItemType = "product", ListGroupItemName = { }, IsDeleted = false });
            listData.Add(new HomeScreenMenuItem { HomeScreenMenuItemId = Guid.NewGuid(), ItemName = "item 4", ItemPosition = 3, GroupName = "", ItemType = "product", ListGroupItemName = { }, IsDeleted = false });
            listData.Add(new HomeScreenMenuItem { HomeScreenMenuItemId = Guid.NewGuid(), ItemName = "item 5", ItemPosition = 4, GroupName = "", ItemType = "product", ListGroupItemName = { }, IsDeleted = false });
            listData.Add(new HomeScreenMenuItem { HomeScreenMenuItemId = Guid.NewGuid(), ItemName = "item 6", ItemPosition = 5, GroupName = "", ItemType = "product", ListGroupItemName = { }, IsDeleted = false });
            listData.Add(new HomeScreenMenuItem { HomeScreenMenuItemId = Guid.NewGuid(), ItemName = "item 7", ItemPosition = 6, GroupName = "", ItemType = "product", ListGroupItemName = { }, IsDeleted = false });
            listData.Add(new HomeScreenMenuItem { HomeScreenMenuItemId = Guid.NewGuid(), ItemName = "item 8", ItemPosition = 7, GroupName = "", ItemType = "product", ListGroupItemName = { }, IsDeleted = false });
            listData.Add(new HomeScreenMenuItem { HomeScreenMenuItemId = Guid.NewGuid(), ItemName = "item 9", ItemPosition = 8, GroupName = "", ItemType = "product", ListGroupItemName = { }, IsDeleted = false });
            listData.Add(new HomeScreenMenuItem { HomeScreenMenuItemId = Guid.NewGuid(), ItemName = "item 10", ItemPosition = 9, GroupName = "", ItemType = "product", ListGroupItemName = { }, IsDeleted = false });
            listData.Add(new HomeScreenMenuItem { HomeScreenMenuItemId = Guid.NewGuid(), ItemName = "item 11", ItemPosition = 10, GroupName = "", ItemType = "product", ListGroupItemName = { }, IsDeleted = false });
            listData.Add(new HomeScreenMenuItem { HomeScreenMenuItemId = Guid.NewGuid(), ItemName = "item 12", ItemPosition = 11, GroupName = "", ItemType = "product", ListGroupItemName = { }, IsDeleted = false });
            listData.Add(new HomeScreenMenuItem { HomeScreenMenuItemId = Guid.NewGuid(), ItemName = "item 13", ItemPosition = 12, GroupName = "", ItemType = "product", ListGroupItemName = { }, IsDeleted = false });


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

            //this.SetLayoutTypeCore(true);

            

            recyclerView.SetAdapter(mAdapter);
        }

        private void SetLayoutTypeCore(bool refreshAdapter)
        {
            if (recyclerView != null)
            {
                if (recyclerView != null)
                {
                    //this.RecyclerView.SetLayoutManager(gridLayoutManager, refreshAdapter);
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

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.edit_menu)
            {
                this.recyclerView.AddOnScrollListener(this.OnScrollListeners);

                var checkPositionPage = this.FindPagePosition(this.recyclerView.GetLayoutManager());

                this.PageIndicatorOnEdit.Visibility = ViewStates.Visible;
                this.PageIndicatorNotEdit.Visibility = ViewStates.Gone;
                this.PageNumberText.Text = checkPositionPage.ToString();

                this.HandlePageIndicator(checkPositionPage);

                var findAddItemMenu = this.DataMenu.FindItem(Resource.Id.add_item);
                findAddItemMenu.SetVisible(true);

                ItemTouchHelper.Callback callback = new ItemMoveCallback(mAdapter, recyclerView, this);
                this._mItemTouchHelper = new ItemTouchHelper(callback);
                this._mItemTouchHelper.AttachToRecyclerView(recyclerView);

                mAdapter.SetMode(true);
                //disini aja set drag and dropnya
                
                recyclerView.SetLayoutManager(new GridLayoutManager(this, 3, GridLayoutManager.Vertical, false));

                mAdapter.NotifyDataSetChanged();

                //disni panggil yang untuk ubah jadi edit modenya
                return true;
            }
            if (id == Resource.Id.save_menu)
            {
                this.recyclerView.AddOnScrollListener(this.OnScrollListeners);

                var checkPositionPage = this.FindPagePosition(this.recyclerView.GetLayoutManager());
                this.HandlePageIndicator(checkPositionPage);

                this.PageIndicatorOnEdit.Visibility = ViewStates.Gone;
                this.PageIndicatorNotEdit.Visibility = ViewStates.Visible;
                var findAddItemMenu = this.DataMenu.FindItem(Resource.Id.add_item);
                findAddItemMenu.SetVisible(false);

                if (this._mItemTouchHelper != null)
                    this._mItemTouchHelper.AttachToRecyclerView(null);

                mAdapter.SetMode(false);

                recyclerView.SetLayoutManager(new GridLayoutManager(this, 4, GridLayoutManager.Horizontal, false));

                mAdapter.NotifyDataSetChanged();
                //disini panggil yg untuk save itemnya

                return true;
            }
            //if (id == Resource.Id.add_item)
            //{
            //    mAdapter.AddItems();
            //    mAdapter.NotifyDataSetChanged();
            //    //disini handle untuk add itemsnya
            //    return true;
            //}

            return base.OnOptionsItemSelected(item);
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
            if (this.DotDictionary != null)
            {
                foreach (Android.Widget.LinearLayout linearlayout in this.DotDictionary)
                    linearlayout.Click -= DotIndicator_Click;
                this.DotDictionary.Clear();
            }
            if (this.recyclerView != null)
                this.recyclerView.RemoveOnScrollListener(this.OnScrollListeners);
            if (this.ImageAddPage != null)
                this.ImageAddPage.Click -= AddPage_Click;
            base.Dispose(disposing);
        }

        public int FindPagePosition(AndroidX.RecyclerView.Widget.RecyclerView.LayoutManager layoutManager)
        {
            GridLayoutManager gridLayoutManager = layoutManager as GridLayoutManager;
            var z = gridLayoutManager.FindLastCompletelyVisibleItemPosition();
            double convertZ = (Convert.ToDouble(z));
            double hasilBagi = (convertZ + 1) / 12;
            int ceilingHasil = (Convert.ToInt32(Math.Ceiling(hasilBagi)));
            //this.MainActivity.HandlePageIndicator(ceilingHasil);
            return ceilingHasil;


            //GridLayoutManager gridLayoutManager = this.layoutManager as GridLayoutManager;
            //var z = gridLayoutManager.FindLastCompletelyVisibleItemPosition();
            //var testZ = gridLayoutManager.FindLastVisibleItemPosition();
            //double convertZ = (Convert.ToDouble(z));
            //double hasilBagi = (convertZ + 1) / 12;
            //int ceilingHasil = (Convert.ToInt32(Math.Ceiling(hasilBagi)));
            //this.MainActivity.HandlePageIndicator(ceilingHasil);
        }

        public void HandlePageIndicator(int PageActive)
        {
            if (this.DotDictionary != null)
            {
                foreach (Android.Widget.LinearLayout linearLayout in this.DotDictionary)
                    linearLayout.Click -= DotIndicator_Click;
                this.DotDictionary.Clear();
            }
            else
            {
                this.DotDictionary = new List<Android.Widget.LinearLayout>();
            }
            this.PinContainer.RemoveAllViewsInLayout();
            double itemCount = (Convert.ToDouble(mAdapter.data.Count) / 12);
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
                this.DotDictionary.Add(linearLayout);
                linearLayout.Tag = i;
                linearLayout.Click += DotIndicator_Click;
            }
            this.PinContainer.RequestLayout();
            this.PinContainer.Invalidate();

        }
        private void DotIndicator_Click(object sender, EventArgs e)
        {
            Android.Widget.LinearLayout view = sender as Android.Widget.LinearLayout;
            int position = (int)view.Tag;
            if (this.recyclerView != null)
                this.recyclerView.SmoothScrollToPosition((position - 1) * 12);
        }
        public void AddPage_Click(object sender, EventArgs e)
        {
            HomeScreenMenu homeScreenMenuUpdate = mAdapter.AddPage(this.homeScreenMenu);
            this.homeScreenMenu = homeScreenMenuUpdate;
            mAdapter.NotifyDataSetChanged();
        }
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
        public override void OnScrollStateChanged(RecyclerView recyclerView, int newState)
        {
            base.OnScrollStateChanged(recyclerView, newState);
            if (newState == RecyclerView.ScrollStateIdle || newState == RecyclerView.ScrollStateSettling)
            {
                //this.MainActivity.ViewModel.UpdateStateScroll(newState);

                GridLayoutManager gridLayoutManager = this.layoutManager as GridLayoutManager;
                var z = gridLayoutManager.FindLastCompletelyVisibleItemPosition();
                var testZ = gridLayoutManager.FindLastVisibleItemPosition();
                double convertZ = (Convert.ToDouble(z));
                double hasilBagi = (convertZ + 1) / 12;
                int ceilingHasil = (Convert.ToInt32(Math.Ceiling(hasilBagi)));
                this.MainActivity.HandlePageIndicator(ceilingHasil);

                if (this.MainActivity.PageNumberText != null)
                    this.MainActivity.PageNumberText.Text = ceilingHasil.ToString();

            }

            if (newState == RecyclerView.ScrollStateDragging || newState == RecyclerView.ScrollStateSettling)
            {
                //this.MainActivity.ViewModel.UpdateStateScroll(1);
            }
            if (newState == RecyclerView.ScrollStateIdle)
            {
                //this.MainActivity.ViewModel.UpdateStateScroll(newState);
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
        private Activity MainActivity { get; set; }
        Android.Widget.LinearLayout GroupingIndicator { get; set; }
        Android.Widget.LinearLayout Container { get; set; }

        public ItemMoveCallback(ItemTouchHelperContract adapter, RecyclerView recyclerView, Activity MainActivity)
        {
            mAdapter = adapter;
            this.RecyclerView = recyclerView;
            this.MainActivity = MainActivity;
        }

        public override bool IsLongPressDragEnabled => base.IsLongPressDragEnabled;
        public override bool IsItemViewSwipeEnabled => base.IsItemViewSwipeEnabled;


        public override int GetMovementFlags(RecyclerView p0, RecyclerView.ViewHolder p1)
        {
            int dragFlags = ItemTouchHelper.Up | ItemTouchHelper.Down | ItemTouchHelper.Start | ItemTouchHelper.End;
            return MakeMovementFlags(dragFlags, 0);
        }

        public override bool OnMove(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder,RecyclerView.ViewHolder target)
        {
            int from = viewHolder.LayoutPosition;
            int to = target.LayoutPosition;

            if (this.dataFrom == -1)
                this.dataFrom = from;
            else
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

            this.DiffX = diffX;
            this.DiffY = diffY;

            if (diffX <= 5 || diffY <= 5)
            {
                List<HomeScreenMenuItem> AdapterData = mAdapter.getDataAdapter();
                var item = AdapterData[target.AdapterPosition];

                //item.isIndicatorShow = true;
                this.ActionDragTodo = false;
                this.Adapter.NotifyItemChanged(target.LayoutPosition);
            }
            else if (diffX > 5 || diffY > 5)
            {
                List<HomeScreenMenuItem> AdapterData = mAdapter.getDataAdapter();
                var item = AdapterData[target.AdapterPosition];

                //item.isIndicatorShow = false;
                this.ActionDragTodo = true;
                this.Adapter.NotifyItemMoved(viewHolder.AdapterPosition, target.AdapterPosition);
            }


            return true;
        }
        public int DiffX { get; set; }
        public int DiffY { get; set; }


        public override void OnSwiped(RecyclerView.ViewHolder p0, int p1)
        {
            throw new NotImplementedException();
        }
        public override void OnSelectedChanged(RecyclerView.ViewHolder viewHolder, int actionState)
        {
            base.OnSelectedChanged(viewHolder, actionState);
            switch (actionState)
            {
                case ItemTouchHelper.ActionStateIdle:
                    if (this.ActionDragTodo)
                    {
                        System.Diagnostics.Debug.Write("Diff X Value Move :" + this.DiffX + "Diff Y Value Move :" + this.DiffY);
                        mAdapter.OnRowMoved(this.dataFrom, this.dataTo);
                    }
                    else
                    {
                        System.Diagnostics.Debug.Write("Diff X Value :" + this.DiffX + "Diff Y Value :" + this.DiffY );
                        mAdapter.OnGrouping(this.dataFrom, this.dataTo);
                        mAdapter.LoadDataAfterGrouping();

                        List<HomeScreenMenuItem> AdapterData = mAdapter.getDataAdapter();
                        var item = AdapterData[this.dataTo];

                        //item.isIndicatorShow = false;
                    }
                    this.Adapter.NotifyItemChanged(this.dataTo);

                    this.dataFrom = -1;
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
}
