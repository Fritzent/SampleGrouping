using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
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
        public List<Data> data;

        public MyAdapter(List<Data> data)
        {
            this.data = data;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            MyViewHolder h = holder as MyViewHolder;
            h.ItemName.Text = data[position].itemName;

        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View v = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_layout, parent, false);
            MyViewHolder holder = new MyViewHolder(v);

            return holder;
        }

        public void onRowMoved(int fromPosition, int toPosition)
        {
            if (fromPosition < toPosition)
            {
                for (int i = fromPosition; i < toPosition; i++)
                {
                    Collections.Swap((IList<object>)data, i, i + 1);
                }
            }
            else
            {
                for (int i = fromPosition; i > toPosition; i--)
                {
                    Collections.Swap((IList<object>)data, i, i - 1);
                }
            }
            NotifyItemMoved(fromPosition, toPosition);
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
            public TextView ItemName;

            public MyViewHolder(View itemView)
                : base(itemView)
            {
                ItemName = itemView.FindViewById<TextView>(Resource.Id.ItemName);
            }
        }
    }
}