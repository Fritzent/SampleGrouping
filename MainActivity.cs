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

        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            //Android.Widget.LinearLayout groupingIndicator = FindViewById<Android.Widget.LinearLayout>(Resource.Id.IndicatorGrouping);
            //this.GroupingIndicator = groupingIndicator;

            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            recyclerView.SetLayoutManager(new GridLayoutManager(this, 4, GridLayoutManager.Horizontal, false));

            PopulateRecyclerView();
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

            ItemTouchHelper.Callback callback = new ItemMoveCallback(mAdapter, recyclerView, this);
            ItemTouchHelper touchHelper = new ItemTouchHelper(callback);
            touchHelper.AttachToRecyclerView(recyclerView);
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
                var findAddItemMenu = this.DataMenu.FindItem(Resource.Id.add_item);
                findAddItemMenu.SetVisible(true);
                mAdapter.SetMode(true);
                
                recyclerView.SetLayoutManager(new GridLayoutManager(this, 3, GridLayoutManager.Vertical, false));

                mAdapter.NotifyDataSetChanged();

                //disni panggil yang untuk ubah jadi edit modenya
                return true;
            }
            if (id == Resource.Id.save_menu)
            {
                var findAddItemMenu = this.DataMenu.FindItem(Resource.Id.add_item);
                findAddItemMenu.SetVisible(false);
                mAdapter.SetMode(false);

                recyclerView.SetLayoutManager(new GridLayoutManager(this, 4, GridLayoutManager.Horizontal, false));

                mAdapter.NotifyDataSetChanged();
                //disini panggil yg untuk save itemnya

                return true;
            }
            if (id == Resource.Id.add_item)
            {
                mAdapter.AddItems();
                mAdapter.NotifyDataSetChanged();
                //disini handle untuk add itemsnya
                return true;
            }

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
