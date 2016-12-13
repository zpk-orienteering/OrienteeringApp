package com.lukaszbalukin.orienteering.utils;

import android.content.Context;
import android.view.View;
import android.widget.Toast;

public class ToastHelper
{
    public static void ShowToast(String message, Context context)
    {    	
    	CharSequence text = message;
    	int duration = Toast.LENGTH_SHORT;

    	Toast toast = Toast.makeText(context, text, duration);
    	toast.show();
    }
}