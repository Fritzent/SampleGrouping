using System;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AndroidX.Core.View;
using AndroidX.RecyclerView.Widget;

namespace SampleGrouping.CustomeView
{
    public interface SnapBlockCallback
    {
        void onBlockSnap(int snapPosition);

        void onBlockSnapped(int snapPosition);
    }
    public class SnapToBlock : SnapHelper
    {
        private static float MILLISECONDS_PER_INCH = 100f;

        private RecyclerView mRecyclerView;
        // Total number of items in a block of view in the RecyclerView
        private int mBlocksize;

        // Maximum number of positions to move on a fling.
        private int mMaxPositionsToMove;

        // Width of a RecyclerView item if orientation is horizonal; height of the item if vertical
        private int mItemDimension;

        // Maxim blocks to move during most vigorous fling.
        private static int mMaxFlingBlocks;

        // Callback interface when blocks are snapped.
        private SnapBlockCallback mSnapBlockCallback;

        // When snapping, used to determine direction of snap.
        private int mPriorFirstPosition = RecyclerView.NoPosition;

        // Our private scroller
        private Scroller mScroller;

        // Horizontal/vertical layout helper
        private OrientationHelper mOrientationHelper;

        // LTR/RTL helper
        private LayoutDirectionHelper mLayoutDirectionHelper;
        public MainActivity MainActivity { get; set; }

        public class CustomInterpolator : Java.Lang.Object, IInterpolator
        {
            public float GetInterpolation(float t)
            {
                t -= 1.0f;
                return t * t * t + 1.0f;
            }
        }

        //private static readonly Interpolator sInterpolator = new CustomInterpolator()
        //{
        //    public float getInterpolation(float t)
        //{
        //    // _o(t) = t * t * ((tension + 1) * t + tension)
        //    // o(t) = _o(t - 1) + 1
        //    t -= 1.0f;
        //    return t * t * t + 1.0f;
        //}
        //};

        public SnapToBlock(int maxFlingBlocks, MainActivity MainActivity) : base()
        {
            mMaxFlingBlocks = maxFlingBlocks;
            this.MainActivity = MainActivity;
        }

        public override void AttachToRecyclerView(RecyclerView recyclerView)
        {
            if (recyclerView != null)
            {
                mRecyclerView = recyclerView;

                LinearLayoutManager layoutManager = (LinearLayoutManager)recyclerView.GetLayoutManager();
                if (layoutManager.CanScrollHorizontally())
                {
                    mOrientationHelper = OrientationHelper.CreateHorizontalHelper(layoutManager);
                    mLayoutDirectionHelper = new LayoutDirectionHelper(ViewCompat.GetLayoutDirection(mRecyclerView), this, this.MainActivity);
                }
                else if (layoutManager.CanScrollVertically())
                {
                    mOrientationHelper = OrientationHelper.CreateVerticalHelper(layoutManager);
                    // RTL doesn't matter for vertical scrolling for this class.
                    mLayoutDirectionHelper = new LayoutDirectionHelper(0, this, this.MainActivity);
                }
                else
                {
                }
                mScroller = new Scroller(mRecyclerView.Context, new CustomInterpolator());
                //mScroller = new Scroller(mRecyclerView.Context);
                initItemDimensionIfNeeded(layoutManager);
            }
            base.AttachToRecyclerView(recyclerView);
        }

        public override int[] CalculateDistanceToFinalSnap(RecyclerView.LayoutManager layoutManager, View targetView)
        {
            int[] outs = new int[2];

            int targetPosition = layoutManager.GetPosition(targetView);
            if (layoutManager.CanScrollHorizontally())
            {
                outs[0] = mLayoutDirectionHelper.getScrollToAlignView(targetView, targetPosition);
            }
            if (layoutManager.CanScrollVertically())
            {
                outs[1] = mLayoutDirectionHelper.getScrollToAlignView(targetView, targetPosition);
            }
            if (mSnapBlockCallback != null)
            {
                if (outs[0] == 0 && outs[1] == 0)
                {
                    mSnapBlockCallback.onBlockSnapped(layoutManager.GetPosition(targetView));
                }
                else
                {
                    mSnapBlockCallback.onBlockSnap(layoutManager.GetPosition(targetView));
                }
            }
            return outs;
        }

        public override View FindSnapView(RecyclerView.LayoutManager layoutManager)
        {
            // Snap to a view that is either 1) toward the bottom of the data and therefore on screen,
            // or, 2) toward the top of the data and may be off-screen.
            int snapPos = calcTargetPosition((LinearLayoutManager)layoutManager);
            View snapView = (snapPos == RecyclerView.NoPosition)
                ? null : layoutManager.FindViewByPosition(snapPos);

            return snapView;
        }

        public override int FindTargetSnapPosition(RecyclerView.LayoutManager layoutManager, int velocityX, int velocityY)
        {
            LinearLayoutManager lm = (LinearLayoutManager)layoutManager;

            initItemDimensionIfNeeded(layoutManager);
            mScroller.Fling(0, 0, velocityX, velocityY, Int32.MinValue, Int32.MaxValue,
                            Int32.MinValue, Int32.MaxValue);

            if (velocityX != 0)
            {
                return mLayoutDirectionHelper.getPositionsToMove(lm, mScroller.FinalX, mItemDimension);
            }

            if (velocityY != 0)
            {
                return mLayoutDirectionHelper.getPositionsToMove(lm, mScroller.FinalY, mItemDimension);
            }

            return RecyclerView.NoPosition;
        }

        public override bool OnFling(int velocityX, int velocityY)
        {
            float MAX_VELOCITY_X = ViewConfiguration.Get(mRecyclerView.Context).ScaledMaximumFlingVelocity;
            float velocityPercentX = velocityX / MAX_VELOCITY_X;
            int normalizedVelocityX = (int)(velocityPercentX * 10000);
            if (velocityX > normalizedVelocityX)
            {
                base.OnFling(4000, velocityY);
                return true;
            }
            else if (velocityX < -normalizedVelocityX)
            {
                base.OnFling(-3000, -velocityY);
                return true;
            }
            else
            {
                return false;
            }
        }


        private int calcTargetPosition(LinearLayoutManager layoutManager)
        {
            int snapPos;
            int firstVisiblePos = layoutManager.FindFirstVisibleItemPosition();

            if (firstVisiblePos == RecyclerView.NoPosition)
            {
                return RecyclerView.NoPosition;
            }
            initItemDimensionIfNeeded(layoutManager);
            if (firstVisiblePos == 0 && mPriorFirstPosition == 0)
            {
                return 0;
            }
            else if (firstVisiblePos >= mPriorFirstPosition)
            {
                // Scrolling toward bottom of data
                int firstCompletePosition = layoutManager.FindFirstCompletelyVisibleItemPosition();
                if (firstCompletePosition != RecyclerView.NoPosition
                    && firstCompletePosition % mBlocksize == 0)
                {
                    snapPos = firstCompletePosition;
                }
                else
                {
                    snapPos = roundDownToBlockSize(firstVisiblePos + mBlocksize);
                }
            }
            else
            {
                // Scrolling toward top of data
                snapPos = roundDownToBlockSize(firstVisiblePos);
                // Check to see if target view exists. If it doesn't, force a smooth scroll.
                // SnapHelper only snaps to existing views and will not scroll to a non-existant one.
                // If limiting fling to single block, then the following is not needed since the
                // views are likely to be in the RecyclerView pool.
                if (layoutManager.FindViewByPosition(snapPos) == null)
                {
                    int[] toScroll = mLayoutDirectionHelper.calculateDistanceToScroll(layoutManager, snapPos);
                    mRecyclerView.SmoothScrollBy(toScroll[0], toScroll[1], new CustomInterpolator());
                    //mRecyclerView.SmoothScrollBy(toScroll[0], toScroll[1], sInterpolator);
                }
            }
            mPriorFirstPosition = firstVisiblePos;

            return snapPos;
        }

        private void initItemDimensionIfNeeded(RecyclerView.LayoutManager layoutManager)
        {
            if (mItemDimension != 0)
            {
                return;
            }

            View child;
            if ((child = layoutManager.GetChildAt(0)) == null)
            {
                return;
            }

            if (layoutManager.CanScrollHorizontally())
            {
                mItemDimension = child.Width;
                mBlocksize = getSpanCount(layoutManager) * (mRecyclerView.Width / mItemDimension);
            }
            else if (layoutManager.CanScrollVertically())
            {
                mItemDimension = child.Height;
                mBlocksize = getSpanCount(layoutManager) * (mRecyclerView.Height / mItemDimension);
            }
            mMaxPositionsToMove = mBlocksize * mMaxFlingBlocks;
        }

        private int getSpanCount(RecyclerView.LayoutManager layoutManager)
        {
            return (layoutManager is GridLayoutManager) ? ((GridLayoutManager)layoutManager).SpanCount : 1;
        }

        private int roundDownToBlockSize(int trialPosition)
        {
            return trialPosition - trialPosition % mBlocksize;
        }

        private int roundUpToBlockSize(int trialPosition)
        {
            return roundDownToBlockSize(trialPosition + mBlocksize - 1);
        }

        public class CustomLinearSmoothScroller : LinearSmoothScroller
        {
            private SnapToBlock SnapToBlock;

            public CustomLinearSmoothScroller(Context context, SnapToBlock snapToBlock) : base(context)
            {
                this.SnapToBlock = snapToBlock;
            }

            public CustomLinearSmoothScroller(IntPtr javaReference, JniHandleOwnership transfer)
                : base(javaReference, transfer)
            {
            }

            protected override void OnTargetFound(View targetView, RecyclerView.State state, Action action)
            {
                int[] snapDistances = this.SnapToBlock.CalculateDistanceToFinalSnap(this.SnapToBlock.mRecyclerView.GetLayoutManager(),
                                                                     targetView);
                int dx = snapDistances[0];
                int dy = snapDistances[1];
                int time = this.CalculateTimeForDeceleration(Math.Max(Math.Abs(dx), Math.Abs(dy)));
                if (time > 0)
                {
                    //action.Update(dx, dy, time);
                    action.Update(dx, dy, time, new CustomInterpolator());
                }
            }

            protected override float CalculateSpeedPerPixel(DisplayMetrics displayMetrics)
            {
                int densityDpi = (int)(displayMetrics.Density * 160f);
                return MILLISECONDS_PER_INCH / densityDpi;
            }
        }

        protected LinearSmoothScroller createScroller(RecyclerView.LayoutManager layoutManager)
        {
            if (!(layoutManager is RecyclerView.SmoothScroller.IScrollVectorProvider))
            {
                return null;
            }

            return new CustomLinearSmoothScroller(mRecyclerView.Context, this);
        }

        public void setSnapBlockCallback(SnapBlockCallback callback)
        {
            mSnapBlockCallback = callback;
        }

        private class LayoutDirectionHelper
        {
            public SnapToBlock SnapToBlock { get; set; }
            // Is the layout an RTL one?
            private bool mIsRTL;
            public MainActivity MainActivity { get; set; }

            public LayoutDirectionHelper(int direction, SnapToBlock snapToBlock, MainActivity mainActivity)
            {
                mIsRTL = direction == 1;
                this.SnapToBlock = snapToBlock;
                this.MainActivity = mainActivity;
            }

            public int dpToPx(int dp)
            {
                //DisplayMetrics displayMetrics = getContext().getResources().getDisplayMetrics();
                DisplayMetrics displayMetrics = this.MainActivity.Resources.DisplayMetrics;
                //return Math.round(dp * (displayMetrics.xdpi / DisplayMetrics.DENSITY_DEFAULT));  
                return (int)Math.Round(dp * (displayMetrics.Xdpi / (float)DisplayMetricsDensity.Default));
            }

            /*
                Calculate the amount of scroll needed to align the target view with the layout edge.
             */
            public int getScrollToAlignView(View targetView, int position)
            {
                int padding = dpToPx(4);
                int decoratedStart = this.SnapToBlock.mOrientationHelper.GetDecoratedStart(targetView);
                int decoratedEnd = this.SnapToBlock.mOrientationHelper.GetDecoratedEnd(targetView);
                int width = this.SnapToBlock.mRecyclerView.Width;

                if (position == 0)
                    padding = 0;

                if (position == 0)
                    padding = dpToPx(2);
                else
                {
                    //padding = 8;
                    padding = dpToPx(7);
                }

                return (mIsRTL)
                    ? this.SnapToBlock.mOrientationHelper.GetDecoratedEnd(targetView) - this.SnapToBlock.mRecyclerView.Width
                    : decoratedStart + (padding * 1);
            }

            /**
             * Calculate the distance to final snap position when the view corresponding to the snap
             * position is not currently available.
             *
             * @param layoutManager LinearLayoutManager or descendent class
             * @param targetPos     - Adapter position to snap to
             * @return int[2] {x-distance in pixels, y-distance in pixels}
             */
            public int[] calculateDistanceToScroll(LinearLayoutManager layoutManager, int targetPos)
            {
                int[] outs = new int[2];

                int firstVisiblePos;

                firstVisiblePos = layoutManager.FindFirstVisibleItemPosition();
                if (layoutManager.CanScrollHorizontally())
                {
                    if (targetPos <= firstVisiblePos)
                    { // scrolling toward top of data
                        if (mIsRTL)
                        {
                            View lastView = layoutManager.FindViewByPosition(layoutManager.FindLastVisibleItemPosition());
                            outs[0] = this.SnapToBlock.mOrientationHelper.GetDecoratedEnd(lastView)
                            + (firstVisiblePos - targetPos) * this.SnapToBlock.mItemDimension;
                        }
                        else
                        {
                            View firstView = layoutManager.FindViewByPosition(firstVisiblePos);
                            //int rowCount = 3;
                            int rowCount = 4;

                            outs[0] = this.SnapToBlock.mOrientationHelper.GetDecoratedStart(firstView)
                            - (((firstVisiblePos - targetPos) / rowCount) * this.SnapToBlock.mItemDimension);
                        }
                    }
                }
                if (layoutManager.CanScrollVertically())
                {
                    if (targetPos <= firstVisiblePos)
                    { // scrolling toward top of data
                        View firstView = layoutManager.FindViewByPosition(firstVisiblePos);
                        outs[1] = firstView.Top - (firstVisiblePos - targetPos) * this.SnapToBlock.mItemDimension;
                    }
                }

                return outs;
            }

            /*
                Calculate the number of positions to move in the RecyclerView given a scroll amount
                and the size of the items to be scrolled. Return integral multiple of mBlockSize not
                equal to zero.
             */
            public int getPositionsToMove(LinearLayoutManager llm, int scroll, int itemSize)
            {
                int positionsToMove;

                positionsToMove = this.SnapToBlock.roundUpToBlockSize(Math.Abs(scroll) / itemSize);

                if (positionsToMove < this.SnapToBlock.mBlocksize)
                {
                    // Must move at least one block
                    positionsToMove = this.SnapToBlock.mBlocksize;
                }
                else if (positionsToMove > this.SnapToBlock.mMaxPositionsToMove)
                {
                    // Clamp number of positions to move so we don't get wild flinging.
                    positionsToMove = this.SnapToBlock.mMaxPositionsToMove;
                }

                if (scroll < 0)
                {
                    positionsToMove *= -1;
                }
                if (mIsRTL)
                {
                    positionsToMove *= -1;
                }

                if (this.SnapToBlock.mLayoutDirectionHelper.isDirectionToBottom(scroll < 0))
                {
                    // Scrolling toward the bottom of data.
                    return this.SnapToBlock.roundDownToBlockSize(llm.FindFirstVisibleItemPosition()) + positionsToMove;
                }
                // Scrolling toward the top of the data.
                return this.SnapToBlock.roundDownToBlockSize(llm.FindLastVisibleItemPosition()) + positionsToMove;
            }

            bool isDirectionToBottom(bool velocityNegative)
            {
                //noinspection SimplifiableConditionalExpression
                return mIsRTL ? velocityNegative : !velocityNegative;
            }
        }
    }
}