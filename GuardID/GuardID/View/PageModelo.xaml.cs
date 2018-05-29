﻿using GuardID.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace GuardID.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PageModelo : ContentPage
	{
		public PageModelo(ZXingBarcodeImageView barcode)
		{
			InitializeComponent ();
            qrResult.Content = barcode;
		}

	}
}