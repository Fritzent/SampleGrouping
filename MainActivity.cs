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
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class Data 
    { 
        public string itemName { get; set; }
        public int itemPosition { get; set; }
    }

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

            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            recyclerView.SetLayoutManager(new GridLayoutManager(this, 4, GridLayoutManager.Horizontal, false));

            PopulateRecyclerView();
        }

        public void PopulateRecyclerView()
        {
            listData.Add(new Data { itemName = "item 1", itemPosition = 1 });
            listData.Add(new Data { itemName = "item 2", itemPosition = 2 });
            listData.Add(new Data { itemName = "item 3", itemPosition = 3 });
            listData.Add(new Data { itemName = "item 4", itemPosition = 4 });
            listData.Add(new Data { itemName = "item 5", itemPosition = 5 });
            listData.Add(new Data { itemName = "item 6", itemPosition = 6 });
            listData.Add(new Data { itemName = "item 7", itemPosition = 7 });
            listData.Add(new Data { itemName = "item 8", itemPosition = 8 });
            listData.Add(new Data { itemName = "item 9", itemPosition = 9 });
            listData.Add(new Data { itemName = "item 10", itemPosition = 10 });

            mAdapter = new MyAdapter(listData);

            ItemTouchHelper.Callback callback = new ItemMoveCallback(mAdapter);
            ItemTouchHelper touchHelper = new ItemTouchHelper(callback);
            touchHelper.AttachToRecyclerView(recyclerView);

            recyclerView.SetAdapter(mAdapter);
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
            View view = (View) sender;
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

        public ItemMoveCallback(ItemTouchHelperContract adapter)
        {
            mAdapter = adapter;
        }

        public override bool IsLongPressDragEnabled => base.IsLongPressDragEnabled;
        public override bool IsItemViewSwipeEnabled => base.IsItemViewSwipeEnabled;


        public override int GetMovementFlags(RecyclerView p0, RecyclerView.ViewHolder p1)
        {
            int dragFlags = ItemTouchHelper.Up | ItemTouchHelper.Down;
            return MakeMovementFlags(dragFlags, 0);
        }

        public override bool OnMove(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder,RecyclerView.ViewHolder target)
        {
            mAdapter.onRowMoved(viewHolder.AdapterPosition, target.AdapterPosition);
            return true;
        }

        public override void OnSwiped(RecyclerView.ViewHolder p0, int p1)
        {
            throw new NotImplementedException();
        }
        public interface ItemTouchHelperContract
        {
            public void onRowMoved(int fromPosition, int toPosition);
        }
    }
}
