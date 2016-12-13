package com.lukaszbalukin.orienteering.utils;

import java.lang.ref.WeakReference;

import com.google.zxing.WriterException;
import com.google.zxing.common.BitMatrix;
import com.google.zxing.qrcode.QRCodeWriter;
import pl.abbd.zpkorienteering.R;

import android.graphics.Bitmap;
import android.os.AsyncTask;
import android.util.Log;
import android.widget.ImageView;
import android.widget.ProgressBar;

public class QRGeneratorTask extends AsyncTask<String, Void, Bitmap>
{
	private final WeakReference<ImageView> imageViewReference;
	private final WeakReference<ProgressBar> progressReference;
	private final String stringToEncode;

	public QRGeneratorTask(ImageView img, ProgressBar prog, String stringToEncode)
	{
		imageViewReference = new WeakReference<ImageView>(img);
		progressReference = new WeakReference<ProgressBar>(prog);
		this.stringToEncode = stringToEncode;
	}
	@Override
	protected void onPostExecute(Bitmap result)
	{
		if(result != null && imageViewReference != null && progressReference != null)
		{
			imageViewReference.get().setImageBitmap(result);
			progressReference.get().setVisibility(4);
			imageViewReference.get().setVisibility(0);
		}
		super.onPostExecute(result);
	}

	@Override
	protected Bitmap doInBackground(String... arg0)
	{
		try
		{
			QRCodeWriter writer = new QRCodeWriter();
			BitMatrix matrix = writer.encode(stringToEncode, com.google.zxing.BarcodeFormat.QR_CODE, 800, 800);
			int width = matrix.getWidth();
		    int height = matrix.getHeight();
		    int[] pixels = new int[width * height];
		    for (int y = 0; y < height; y++) 
		    {
		    	int offset = y * width;
		    	for (int x = 0; x < width; x++) 
		    	{
		    		pixels[offset + x] = matrix.get(x, y) ? 0xFF000000 : 0xFFAAAAAA;
		    	}
		    }
		    	
		    Bitmap bitmap = Bitmap.createBitmap(width, height, Bitmap.Config.ARGB_8888);
    	    bitmap.setPixels(pixels, 0, width, 0, 0, width, height);
    	        	    
    	    return bitmap;
		
		}
		catch (WriterException e)
		{
			Log.e("EXPORTER", "Error creating qr code");
			return null;
		}
	}
	
	

}
