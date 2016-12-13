package com.lukaszbalukin.orienteering.services;

import java.io.File;
import java.io.IOException;
import java.util.Date;
import java.util.Timer;
import java.util.TimerTask;
import java.util.concurrent.ScheduledThreadPoolExecutor;
import java.util.concurrent.TimeUnit;

import com.lukaszbalukin.orienteering.utils.ToastHelper;

import android.app.Service;
import android.content.Intent;
import android.os.IBinder;
import android.util.Log;
import android.widget.Toast;

public class TimeWatcherService extends Service
{
	private static Boolean isRunning = false;
	
	public static Boolean GetIsRunning()
	{
		return isRunning;
	}	
	
	ScheduledThreadPoolExecutor timer;
	@Override
	public void onCreate()
	{		
		// TODO Auto-generated method stub
		super.onCreate();
						
		timer = new ScheduledThreadPoolExecutor(5);
		timer.scheduleAtFixedRate(new TimeWatcherTask(this), 0, 10, TimeUnit.SECONDS);
		isRunning = true;
		//ToastHelper.ShowToast("Usługa wystartowała", this);
	}
	
	@Override
	public void onDestroy()
	{
		super.onCreate();
		timer.shutdown();
		timer.purge();
		isRunning = false;
		//ToastHelper.ShowToast("Usługa zakończona", this);
	}

	@Override
	public IBinder onBind(Intent arg0)
	{
		// TODO Auto-generated method stub
		return null;
	}

}
