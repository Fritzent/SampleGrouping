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
    public class MyAdapter : RecyclerView.Adapter, ItemMoveCallback.ItemTouchHelperContract
    {
        #region Fields
        private Dictionary<int, List<Data>> _groupingDictionary;
        private int _targetPositionToShowIndicatorGrouping;
        private bool _isIndicatorGroupingShowForTarget;
        #endregion
        #region Properties
        public List<Data> data;
        public Dictionary<int, List<Data>> GroupDictionary
        {
            get
            {
                if (_groupingDictionary == null)
                    _groupingDictionary = new Dictionary<int, List<Data>>();
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
        #endregion

        public MyAdapter(List<Data> data, MainActivity mainActivity)
        {
            this.data = data;
            this.MainActivity = mainActivity;
        }
        public void SetDataToShowGroupingIndicator (int TargetPosition, bool IsTargetIndicatorGroupingShowed)
        {
            this.TargetPositionToShowIndicatorGrouping = TargetPosition;
            this.IsIndicatorGroupingShowForTarget = IsTargetIndicatorGroupingShowed;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            MyViewHolder h = holder as MyViewHolder;
            
            Data itemProduct = data[position] as Data;
            
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
            }
            //disini handle kalau dia type yg grouping munculin yg bagian grouping
            if (!string.IsNullOrEmpty(itemProduct.groupName) || itemProduct.listGroupItemName != null)
            {
                if (h.GroupItemSection != null)
                {
                    ViewGroup GroupItemSection = h.GroupItemSection as ViewGroup;
                    if (GroupItemSection != null)
                        GroupItemSection.Visibility = ViewStates.Visible;
                    //ini untuk handle berapa jumlah nama yg dimunculin dalam item group sectionnya
                    int countGroupItemNameInListGroupItemName = itemProduct.listGroupItemName.Count();
                    if (itemProduct.listGroupItemName.Count() > 4)
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
                                        if (itemProduct.listGroupItemName[i - 1] != null)
                                        {
                                            GroupItemName.Text = itemProduct.listGroupItemName[i - 1];
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
                                        if (itemProduct.listGroupItemName[i - 1] != null)
                                        {
                                            GroupItemName.Text = itemProduct.listGroupItemName[i - 1];
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
                                        if (itemProduct.listGroupItemName[i - 1] != null)
                                        {
                                            GroupItemName.Text = itemProduct.listGroupItemName[i - 1];
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
                                        if (itemProduct.listGroupItemName[i - 1] != null)
                                        {
                                            GroupItemName.Text = itemProduct.listGroupItemName[i - 1];
                                            GroupItemName.Visibility = ViewStates.Visible;
                                        }
                                    }
                                }
                            }

                            //if (h.Views.ContainsKey("GroupItemItemName"+i))
                            //{
                            //    TextView GroupItemName = h.Views["GroupItemItemName" + i] as TextView;
                            //    if (GroupItemName != null)
                            //    {
                            //        if(itemProduct.listGroupItemName[i - 1] != null)
                            //        {
                            //            GroupItemName.Text = itemProduct.listGroupItemName[i - 1];
                            //            GroupItemName.Visibility = ViewStates.Visible;
                            //        }
                            //    }
                            //}
                        }
                    }
                }
            }
            //disini handle kalau dia type gk grouping munculin yg bagian gk grouping
            if (string.IsNullOrEmpty(itemProduct.groupName) && itemProduct.listGroupItemName == null)
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
                        NonGroupItemName.Text = itemProduct.itemName;
                        NonGroupItemName.Visibility = ViewStates.Visible;
                    }

                }
                //if (h.Views.ContainsKey("NonGroupItemSection"))
                //{
                //    ViewGroup NonGroupItemSection = h.Views["NonGroupItemSection"] as ViewGroup;
                //    if (NonGroupItemSection != null)
                //        NonGroupItemSection.Visibility = ViewStates.Visible;
                //}
                //if (h.Views.ContainsKey("NonGroupItemName"))
                //{
                //    TextView NonGroupItemName = h.Views["NonGroupItemName"] as TextView;

                //}
            }
            //disini untuk handle yg indicator groupingnya
            if (itemProduct.isIndicatorShow)
            {
                if (h.Container != null)
                {
                    FrameLayout Container = h.Container as FrameLayout;
                    if (Container != null)
                    {
                        ViewGroup.LayoutParams param = Container.LayoutParameters;
                        this.previousContainerSize = (int)param.Width;
                        param.Width = (int)(150);
                        Container.LayoutParameters = param;
                        //disini ukurannya diubah jadi 100 dp
                    }
                }
                if (h.ItemContainer != null)
                {
                    FrameLayout ItemContainer = h.ItemContainer as FrameLayout;
                    if (ItemContainer != null)
                    {
                        ViewGroup.LayoutParams param = ItemContainer.LayoutParameters;
                        this.previousItemContainerSize = (int)param.Width;
                        param.Width = (int)(100);
                        ItemContainer.LayoutParameters = param;
                        //disini ukurannya diubah jadi 80dpan
                    }
                }
            }
            else if (!itemProduct.isIndicatorShow && this.previousContainerSize != 0 && this.previousItemContainerSize != 0)
            {

                if (h.Container != null)
                {
                    FrameLayout Container = h.Container as FrameLayout;
                    if (Container != null)
                    {
                        ViewGroup.LayoutParams param = Container.LayoutParameters;
                        param.Width = this.previousContainerSize;
                        Container.LayoutParameters = param;
                        //disini ukurannya diubah jadi semula
                    }
                }
                if (h.ItemContainer != null)
                {
                    FrameLayout ItemContainer = h.ItemContainer as FrameLayout;
                    if (ItemContainer != null)
                    {
                        ViewGroup.LayoutParams param = ItemContainer.LayoutParameters;
                        param.Width = this.previousItemContainerSize;
                        ItemContainer.LayoutParameters = param;
                        //disini ukurannya diubah jadi semula
                    }
                }
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

            if (holder != null)
            {
                Context context = this.MainActivity;

                int fragmentHeight = this.MainActivity.Window.DecorView.Height;
                int fragmentWidth = this.MainActivity.Window.DecorView.Width;
                int cardMargin = dpToPx((int)4);
                int cardElevation = dpToPx((int)4);

                int width = 0;
                int height = 0;
                width = (int)((fragmentWidth / 3) - (8 * 2) - cardElevation - 5);
                height = (int)((fragmentHeight / 4) - (8 * 2) - (cardElevation * 9) - 15);

                //Intersoft.Crosslight.Android.v7.CardView container = holder.ContentView.FindViewById<Intersoft.Crosslight.Android.v7.CardView>(Resource.Id.container);
                AndroidX.CardView.Widget.CardView container = holder.ItemView.FindViewById<AndroidX.CardView.Widget.CardView>(Resource.Id.CardContainer);

                if (container != null)
                {
                    ViewGroup.LayoutParams param = container.LayoutParameters;
                    param.Width = width;
                    param.Height = height;
                    container.LayoutParameters = param;
                }

                //int height = (int)((fragmentHeight / 3) - 5);

            }

            return holder;
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
            //5 - 1
            if (fromPosition < toPosition)
            {
                for (int i = fromPosition; i < (toPosition + 1); i++)
                {
                    IList<object> dataToSwap = this.data.ToList<object>();
                    if (i != (dataToSwap.Count() - 1))
                        Collections.Swap(dataToSwap, i, i + 1);
                    else
                        Collections.Swap(dataToSwap, i, i);
                }
            }
            else
            {
                for (int i = fromPosition; i > (toPosition -1); i--)
                {
                    IList<object> dataToSwap = this.data.ToList<object>();
                    if (i != 0)
                        Collections.Swap(dataToSwap, i, (i - 1));
                    else
                        Collections.Swap(dataToSwap, i, i);
                }
            }
        }

        public void OnGrouping(int fromPosition, int toPosition)
        {
            //var getItemFrom = this.Items.FirstOrDefault(o => o.Position == From);
            //var getFirstItemTo = this.Items.FirstOrDefault(o => o.Position == To);
            //var getItemTo = this.Items.Where(o => o.Position == To);
            var getItemFrom = this.data.FirstOrDefault(o => o.itemPosition == fromPosition);
            var getFirstItemTo = this.data.FirstOrDefault(o => o.itemPosition == toPosition);
            var getItemTo = this.data.Where(o => o.itemPosition == toPosition);
            string GroupName = "Grouping";

            if (getItemFrom != null && getFirstItemTo != null && getItemTo != null)
            {
                if (!string.IsNullOrEmpty(getItemFrom.itemType) && string.IsNullOrEmpty(getItemFrom.groupName))
                {
                    if (!string.IsNullOrEmpty(getFirstItemTo.itemType))
                    {
                        getItemFrom.itemPosition = toPosition;
                        if (getFirstItemTo.groupName != "" || !string.IsNullOrEmpty(getFirstItemTo.groupName))
                            getItemFrom.groupName = getFirstItemTo.groupName;
                        else
                            getItemFrom.groupName = GroupName;

                        foreach (var itemTo in getItemTo)
                        {
                            if (string.IsNullOrEmpty(itemTo.groupName))
                                itemTo.groupName = GroupName;
                        }
                    }
                }
                //if (getItemFrom.ItemType != null && !string.IsNullOrEmpty(getItemFrom.ItemType) && string.IsNullOrEmpty(getItemFrom.GroupName))
                //{
                //    if (getFirstItemTo.ItemType != null && !string.IsNullOrEmpty(getFirstItemTo.ItemType))
                //    {
                //        getItemFrom.Position = To;
                //        //getItemFrom.GroupId = newGroupId;
                //        getItemFrom.ModifiedBy = createdBy;
                //        getItemFrom.ModifiedDate = DateTime.Now;
                //        if (getFirstItemTo.GroupName != "" || !string.IsNullOrEmpty(getFirstItemTo.GroupName))
                //            getItemFrom.GroupName = getFirstItemTo.GroupName;
                //        else
                //            getItemFrom.GroupName = GroupName;

                //        this.Repository.Update(getItemFrom);

                //        foreach (var itemTo in getItemTo)
                //        {
                //            if (itemTo.GroupName == "" || string.IsNullOrEmpty(itemTo.GroupName))
                //            {
                //                itemTo.GroupName = GroupName;
                //                //itemTo.GroupId = newGroupId;
                //                itemTo.ModifiedBy = createdBy;
                //                itemTo.ModifiedDate = DateTime.Now;
                //                this.Repository.Update(itemTo);
                //            }
                //        }
                //    }
                //}
            }

            this.NotifyDataSetChanged();
        }

        public List<Data> getDataAdapter()
        {
            return this.data;
        }

        public bool LoadDataAfterGrouping()
        {
            List<Data> dataSource = this.data.ToList();
            List<Data> newDataGenerated = new List<Data>();

            for(int x = 0; x < dataSource.Count(); x++)
            {
                //List<HomeScreenMenuItem> findItemInResult = homeScreenItemResult.Where(o => o.Position == x && o.IsDeleted == false).ToList();
                List<Data> findItemInDataSource = this.data.Where(o => o.itemPosition == x).ToList();
                
                if (findItemInDataSource.Count() > 0)
                {
                    foreach(Data item in findItemInDataSource)
                    {
                        if (item != null)
                        {
                            if (!string.IsNullOrEmpty(item.groupName))
                            {
                                //List<HomeScreenMenuItem> getGroupItem = homeScreenItemResult.Where(o => o.Position == item.Position).ToList();
                                List<Data> getGroupItemInDataSource = dataSource.Where(o => o.itemPosition == item.itemPosition).ToList();

                                foreach(var eachItem in getGroupItemInDataSource)
                                {
                                    if (!this.GroupDictionary.ContainsKey(eachItem.itemPosition))
                                        this.GroupDictionary.Add(eachItem.itemPosition, getGroupItemInDataSource);
                                    if (this.GroupDictionary.ContainsKey(eachItem.itemPosition))
                                    {
                                        if (!this.GroupDictionary[eachItem.itemPosition].Contains(eachItem))
                                            this.GroupDictionary[eachItem.itemPosition].Add(eachItem);
                                    }
                                }

                                var LastItemInThisGroup = getGroupItemInDataSource.Last();

                                if (item.itemId == LastItemInThisGroup.itemId)
                                {
                                    item.listGroupItemName = new List<string>();

                                    List<Data> itemsInDictionary = this.GroupDictionary[item.itemPosition];

                                    foreach(Data itemInDictionary in itemsInDictionary)
                                    {
                                        if (itemInDictionary.itemId != item.itemId)
                                        {
                                            if (itemInDictionary.itemType == "product")
                                            {
                                                if (itemInDictionary.itemName != null)
                                                    item.listGroupItemName.Add(itemInDictionary.itemName);
                                            }
                                        }
                                        else
                                        {
                                            if (item.itemType == "product")
                                            {
                                                if (item.itemName != null)
                                                    item.listGroupItemName.Add(item.itemName);
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
            }
            this.data = newDataGenerated;
            this.NotifyDataSetChanged();

            return true;
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
            }
        }
    }
}