﻿using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using UniversalHelpers.AwaitableUI;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.Foundation;

namespace UniversalHelpers.Controls
{
    public class CiclePanel : ItemsControl
    {
        public CiclePanel()
            : base()
        {
            this.ItemsPanel = GetItemsPanelTemplate();
            this.LayoutUpdated += CiclePanelView_LayoutUpdated;
        }

        async void CiclePanelView_LayoutUpdated(object sender, object e)
        {
            await this.WaitForLayoutUpdateAsync();
            CalculatePositions();
        }

        private ItemsPanelTemplate GetItemsPanelTemplate()
        {
            string xaml = @"<ItemsPanelTemplate   xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                            <Grid>
                               
                            </Grid>
                    </ItemsPanelTemplate>";
            return XamlReader.Load(xaml) as ItemsPanelTemplate;
        }

        protected async override void OnItemsChanged(object e)
        {

            base.OnItemsChanged(e);
            await this.WaitForLayoutUpdateAsync();
            CalculatePositions();

        }

        private void CalculatePositions()
        {
            if (Items.Count == 0) return;

            double width = this.Width;
            if (double.IsNaN(width))
                width = this.ActualWidth;

            double height = this.Height;
            if (double.IsNaN(height))
                height = ActualHeight;

            width -= GetFrameworkElementFromItem(0).ActualWidth;
            height -= GetFrameworkElementFromItem(0).ActualHeight;

            double degrees = 360 / this.Items.Count;
            double radians = (Math.PI / 180) * degrees;
            double radius = height > width ? width / 2 : height / 2;

            for (int i = 0; i < this.Items.Count; i++)
            {
                FrameworkElement fe = GetFrameworkElementFromItem(i);
                fe.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;
                fe.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center;
                fe.RenderTransform = new CompositeTransform();
                fe.RenderTransformOrigin = new Point(0.5, 0.5);

                CompositeTransform ct = fe.RenderTransform as CompositeTransform;
                ct.TranslateX = radius * Math.Cos(radians * i);
                ct.TranslateY = radius * Math.Sin(radians * i);
            }
            if (AlignRotation)
                FixRotations();
        }

        private void FixRotations()
        {
            if (Items.Count == 0) return;
            double degrees = 360 / this.Items.Count;
            double radians = (Math.PI / 180) * degrees;

            for (int i = 0; i < this.Items.Count; i++)
            {
                FrameworkElement fe = GetFrameworkElementFromItem(i);
                CompositeTransform ct = fe.RenderTransform as CompositeTransform;

                ct.Rotation = degrees * i;
            }
        }

        private FrameworkElement GetFrameworkElementFromItem(int i)
        {
            DependencyObject dob = this.ContainerFromItem(this.Items[i]);
            FrameworkElement fe = null;
            if (VisualTreeHelper.GetChildrenCount(dob) == 0)
                fe = dob as FrameworkElement;
            else
                fe = VisualTreeHelper.GetChild(dob, 0) as FrameworkElement;
            return fe;
        }



        public bool AlignRotation
        {
            get { return (bool)GetValue(AlignRotationProperty); }
            set
            {
                SetValue(AlignRotationProperty, value);
                FixRotations();
            }
        }

        // Using a DependencyProperty as the backing store for AlignRotation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AlignRotationProperty =
            DependencyProperty.Register("AlignRotation", typeof(bool), typeof(CiclePanel), new PropertyMetadata(false));


    }
}
