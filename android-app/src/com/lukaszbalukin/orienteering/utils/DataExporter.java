package com.lukaszbalukin.orienteering.utils;

import java.io.BufferedWriter;
import java.io.File;
import java.io.FileOutputStream;
import java.io.FileWriter;
import java.io.IOException;

import android.os.Environment;
import android.util.Log;

import com.lukaszbalukin.orienteering.models.OrienteeringRun;

public class DataExporter
{
	public void WriteToSD(OrienteeringRun run)
	{
		String fileContents = "";		
		try
		{
			Serializer serializer = new Serializer();
			fileContents = serializer.SerializeRun(run);
			//Log.i("EXPORTER",fileContents);
		} 
		catch (Exception ex)
		{
			Log.e("OrienteeringExporter","Problem z zapisem biegu do XML.");
		}
		
		try 
		{
		    File root = Environment.getExternalStorageDirectory();
		    
		    if (root.canWrite())
		    {
		    	Encryptor crypt = new Encryptor();				
				byte[] encryptedRun = crypt.Encrypt(run.GetRunHeader(), fileContents);
				
		        File runFile = new File(root, "OrienteeringRun.xml");		        
		        FileOutputStream fos = new FileOutputStream(runFile);
		        fos.write(encryptedRun);
		        fos.close();
		    }
		}
		catch (IOException e)
		{
		    Log.e("OrienteeringExporter", "Problem z zapisem XML biegu do pliku. " + e.getMessage(), e);		    
		}
	}

	
}
