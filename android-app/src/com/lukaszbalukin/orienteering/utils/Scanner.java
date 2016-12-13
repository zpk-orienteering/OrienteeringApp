package com.lukaszbalukin.orienteering.utils;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.ActivityNotFoundException;
import android.content.DialogInterface;
import android.content.Intent;
import android.net.Uri;
import android.os.Message;

public class Scanner
{
	private static final int RESULT_OK = -1;

	public void ScanCode(final Activity activity)
	{
		try
		{
			Intent intent = new Intent("com.google.zxing.client.android.SCAN");
			intent.putExtra("SCAN_MODE", "QR_CODE_MODE");
			intent.putExtra("SAVE_HISTORY", false);
			activity.startActivityForResult(intent, 0);
		}
		catch (ActivityNotFoundException ex)
		{
			
			DialogInterface.OnClickListener dialogClickListener = new DialogInterface.OnClickListener() {			    
			    public void onClick(DialogInterface dialog, int which) {
			        switch (which){
			        case DialogInterface.BUTTON_POSITIVE:
			        	Intent intent = new Intent(Intent.ACTION_VIEW);
			        	intent.setData(Uri.parse("market://details?id=com.google.zxing.client.android"));
			        	try
			        	{
			        		activity.startActivity(intent);
			        	}
			        	catch(ActivityNotFoundException x)
			        	{
			        		ToastHelper.ShowToast("Brak dostępu do marketu.", activity);
			        	}
			            break;

			        case DialogInterface.BUTTON_NEGATIVE:
			            dialog.dismiss();
			            break;
			        }
			    }
			};

			AlertDialog.Builder builder = new AlertDialog.Builder(activity);
			builder.setTitle("Nie znaleziono aplikacji skanującej.").setMessage("Czy chcesz pobrać darmowy skaner kodów z Marketu?").setPositiveButton("Tak", dialogClickListener)
			    .setNegativeButton("Nie", dialogClickListener).show();			
			
		}
	}
	
	public String ProcessResults(int requestCode, int resultCode, Intent data)
	{
		if (resultCode == RESULT_OK)
        {					
            String contents = data.getStringExtra("SCAN_RESULT"); //this is the result
            return contents;
        } 
		else
        {
			return "ERROR";
        }
	}
}
