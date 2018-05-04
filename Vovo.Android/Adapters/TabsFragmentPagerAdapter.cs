using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using Java.Lang;

namespace Vovo.Android.Adapters
{
    public class TabsFragmentPagerAdapter : FragmentPagerAdapter
    {
        private readonly Fragment[] fragments;

        private readonly ICharSequence[] titles;

        public TabsFragmentPagerAdapter(FragmentManager fm, Fragment[] fragments, ICharSequence[] titles) : base(fm)
        {
            this.fragments = fragments;
            this.titles = titles;
        }
        public override int Count
        {
            get
            {
                return fragments.Length;
            }
        }

        public override Fragment GetItem(int position)
        {
            return fragments[position];
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            //return titles[position];
            return null;
        }
    }
}