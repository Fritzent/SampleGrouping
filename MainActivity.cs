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
    public class Data 
    { 
        public bool isIndicatorShow { get; set; }
        public int itemId { get; set; }
        public string itemName { get; set; }
        public string itemType { get; set; }
        public int itemPosition { get; set; }
        public List<string> listGroupItemName { get; set; }
        public string groupName { get; set; }
    }

    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private RecyclerView recyclerView;
        private MyAdapter mAdapter;
        List<Data> listData = new List<Data>();
        
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
            recyclerView.SetLayoutManager(new GridLayoutManager(this, 3, GridLayoutManager.Vertical, false));

            PopulateRecyclerView();
        }

        public void PopulateRecyclerView()
        {
            listData.Add(new Data { itemId = 1, itemName = "item 1", itemPosition = 0, groupName = "", itemType= "product", listGroupItemName = { } });
            listData.Add(new Data { itemId = 2, itemName = "item 2", itemPosition = 1, groupName = "", itemType = "product", listGroupItemName = { } });
            listData.Add(new Data { itemId = 3, itemName = "item 3", itemPosition = 2, groupName = "", itemType = "product", listGroupItemName = { } });
            listData.Add(new Data { itemId = 4, itemName = "item 4", itemPosition = 3, groupName = "", itemType = "product", listGroupItemName = { } });
            listData.Add(new Data { itemId = 5, itemName = "item 5", itemPosition = 4, groupName = "", itemType = "product", listGroupItemName = { } });
            listData.Add(new Data { itemId = 6, itemName = "item 6", itemPosition = 5, groupName = "", itemType = "product", listGroupItemName = { } });
            listData.Add(new Data { itemId = 7, itemName = "item 7", itemPosition = 6, groupName = "", itemType = "product", listGroupItemName = { } });
            listData.Add(new Data { itemId = 8, itemName = "item 8", itemPosition = 7, groupName = "", itemType = "product", listGroupItemName = { } });
            listData.Add(new Data { itemId = 9, itemName = "item 9", itemPosition = 8, groupName = "", itemType = "product", listGroupItemName = { } });
            listData.Add(new Data { itemId = 10, itemName = "item 10", itemPosition = 9, groupName = "", itemType = "product", listGroupItemName = { } });
            listData.Add(new Data { itemId = 11, itemName = "item 11", itemPosition = 10, groupName = "", itemType = "product", listGroupItemName = { } });
            listData.Add(new Data { itemId = 12, itemName = "item 12", itemPosition = 11, groupName = "", itemType = "product", listGroupItemName = { } });

            mAdapter = new MyAdapter(listData, this);

            ItemTouchHelper.Callback callback = new ItemMoveCallback(mAdapter, recyclerView, this);
            ItemTouchHelper touchHelper = new ItemTouchHelper(callback);
            touchHelper.AttachToRecyclerView(recyclerView);

            recyclerView.SetAdapter(mAdapter);
        }

        private void SetLayoutTypeCore(bool refreshAdapter)
        {
            if (recyclerView != null)
            {
                //this.RecyclerView.ItemLayoutId = Resource.Layout.product_list_grid_item_layout;

                //GridLayoutManager gridLayoutManager = new GridLayoutManager(this.Activity, 3, GridLayoutManager.Horizontal, false);

                if (recyclerView != null)
                {
                    //this.RecyclerView.SetLayoutManager(gridLayoutManager, refreshAdapter);
                    CustomeView.SnapToBlock snapToBlock = new CustomeView.SnapToBlock(12, this);
                    snapToBlock.AttachToRecyclerView(recyclerView);
                }
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
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
                List<Data> AdapterData = mAdapter.getDataAdapter();
                var item = AdapterData[target.AdapterPosition];

                item.isIndicatorShow = true;
                this.ActionDragTodo = false;
                this.Adapter.NotifyItemChanged(target.LayoutPosition);
            }
            else if (diffX > 5 || diffY > 5)
            {
                List<Data> AdapterData = mAdapter.getDataAdapter();
                var item = AdapterData[target.AdapterPosition];

                item.isIndicatorShow = false;
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

                        List<Data> AdapterData = mAdapter.getDataAdapter();
                        var item = AdapterData[this.dataTo];

                        item.isIndicatorShow = false;
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
            public List<Data> getDataAdapter();
        }
    }
}
