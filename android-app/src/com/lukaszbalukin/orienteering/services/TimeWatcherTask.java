package com.lukaszbalukin.orienteering.services;

import java.io.BufferedWriter;
import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
import java.io.PrintWriter;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.TimerTask;

import android.content.Context;
import android.os.SystemClock;
import android.util.Log;

public class TimeWatcherTask implements Runnable
{
	Context ctx;
	
	public TimeWatcherTask(TimeWatcherService timeWatcherService)
	{
		ctx = timeWatcherService.getApplicationContext();
	}

	public void run()
	{		
		File timeFile = new File(ctx.getFilesDir(), "time.log");
		
		try
		{			
			//Log.w("TAG",new Date().toString() + SystemClock.elapsedRealtime());		
		    PrintWriter out = new PrintWriter(new BufferedWriter(new FileWriter(timeFile, true)));
		    out.println(new Date().getTime());
		    out.close();
		} catch (IOException e)
		{
		    
		}
	}

}
