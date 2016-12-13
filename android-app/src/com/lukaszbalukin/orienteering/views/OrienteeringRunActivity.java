package com.lukaszbalukin.orienteering.views;


import java.io.BufferedReader;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStreamReader;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Currency;
import java.util.Date;
import java.util.Random;
import java.util.Timer;
import java.util.TimerTask;

import javax.xml.parsers.ParserConfigurationException;

import org.xml.sax.SAXException;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.ActivityNotFoundException;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.pm.ApplicationInfo;
import android.content.pm.PackageManager;
import android.net.Uri;
import android.os.Bundle;
import android.os.Environment;
import android.os.SystemClock;
import android.preference.PreferenceManager;
import android.test.UiThreadTest;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.Chronometer;
import android.widget.HorizontalScrollView;
import android.widget.LinearLayout;
import android.widget.TextView;

import com.google.zxing.common.detector.MathUtils;
import pl.abbd.zpkorienteering.R;
import com.lukaszbalukin.orienteering.controllers.OrienteeringRunController;
import com.lukaszbalukin.orienteering.models.OrienteeringRun;
import com.lukaszbalukin.orienteering.services.TimeWatcherService;
import com.lukaszbalukin.orienteering.utils.DataExporter;
import com.lukaszbalukin.orienteering.utils.DatabaseHelper;
import com.lukaszbalukin.orienteering.utils.Scanner;
import com.lukaszbalukin.orienteering.utils.Serializer;
import com.lukaszbalukin.orienteering.utils.ToastHelper;

/**
 * @author lukasz
 * 
 * Główny ekran biegu.
 */

public class OrienteeringRunActivity extends Activity
{
	public static final String TAG = "OrienteeringRunActivity";
	private static final int SET_USER_ID_CODE = 10;
	
	Scanner scanner;	
	OrienteeringRunController controller;
	Timer uiRefreshTimer;
	SimpleDateFormat sdfLong = new SimpleDateFormat("dd-MM-yyyy HH:mm");
	SimpleDateFormat sdfShort = new SimpleDateFormat("HH:mm");
	
	@Override
	public void onCreate(Bundle savedInstanceState)
	{
		super.onCreate(savedInstanceState);
		setContentView(R.layout.orienteering_run);
		
		CheckIfScannerPresent();
		
		scanner = new Scanner();
		controller = new OrienteeringRunController(this);
		
		
		TextView txtId = (TextView)findViewById(R.id.txtRunnerId);
		SharedPreferences sharedPref = PreferenceManager.getDefaultSharedPreferences(this);
		int id = sharedPref.getInt("UserID", -1544);
		if(id == -1544)
		{
			txtId.setText("Ustaw identyfikator przed rozpoczęciem biegu");
		}
		else
		{
			txtId.setText("Twój identyfikator:  " + id);
		}
		
		
		
	}

	private void CheckIfScannerPresent()
	{
		try
		{
		    ApplicationInfo info = getPackageManager().
		            getApplicationInfo("com.google.zxing.client.android", 0 );
		    
		    // Zwróciło info - czyli jest
		}
		catch(PackageManager.NameNotFoundException e )
		{
		    // Wyjątek - nie ma skanera.
			DialogInterface.OnClickListener dialogClickListener = new DialogInterface.OnClickListener() {			    
			    public void onClick(DialogInterface dialog, int which) {
			        switch (which){
			        case DialogInterface.BUTTON_POSITIVE:
			        	Intent intent = new Intent(Intent.ACTION_VIEW);
			        	intent.setData(Uri.parse("market://details?id=com.google.zxing.client.android"));
			        	try
			        	{
			        		startActivity(intent);
			        	}
			        	catch(ActivityNotFoundException x)
			        	{
			        		ToastHelper.ShowToast("Brak dostępu do marketu.", getApplicationContext());
			        	}			        	
			        	
			            break;

			        case DialogInterface.BUTTON_NEGATIVE:
			            dialog.dismiss();
			            break;
			        }
			    }
			};

			AlertDialog.Builder builder = new AlertDialog.Builder(this);
			builder.setTitle("Nie znaleziono aplikacji skanującej.").setMessage("Czy chcesz pobrać darmowy skaner kodów z Marketu?").setPositiveButton("Tak", dialogClickListener)
			    .setNegativeButton("Nie", dialogClickListener).show();	
		}
		
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu)
	{
		getMenuInflater().inflate(R.menu.activity_main, menu);
		return true;
	}
	
	@Override
	public boolean onOptionsItemSelected(MenuItem item)
	{
		switch (item.getItemId()) 
		{
	        case R.id.menu_settings:
	            controller.ShowOptions(this);
	            break;
	        case R.id.menu_history:
	        	controller.ShowHistory(this);
	        	break;
	        case R.id.menu_reset:
	        	ResetRunDialog();
	        	break;
	        case R.id.menu_about:
	        	Intent i = new Intent(this, AboutMe.class);
	    		this.startActivity(i);
	        	break;
	        case R.id.menu_setid:
	        	if(controller.GetOrienteeringRun() != null &&
	        		controller.GetOrienteeringRun().GetIsInProgress() == true)
	        		{
	        			ToastHelper.ShowToast("Nie możesz zmieniać identyfikatora podczas biegu.", getApplicationContext());
	        		}
	        		else
	        		{
	        			startActivityForResult(new Intent(this, UserIdentifierChooser.class), SET_USER_ID_CODE);
	        		}	        	
	        	break;
	        	
	        default:
	            return super.onOptionsItemSelected(item);
		}
		return super.onOptionsItemSelected(item);
	}
	
	public void scan(View view)
	{
		SharedPreferences sharedPref = PreferenceManager.getDefaultSharedPreferences(this);
		boolean isDebug = sharedPref.getBoolean("IsDebugEnabled", false);
		
		if(isDebug == true)
		{
			controller.ProcessCodeString("ST/asd");
			controller.ProcessCodeString("CH/1");
			controller.ProcessCodeString("CH/2");
			controller.ProcessCodeString("FI/xtra");			
		}
		else
		{
			scanner.ScanCode(this);
		}			
	}
	
	
	
	@Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        
        if (requestCode == 0)
        {
        	String codeString = scanner.ProcessResults(requestCode, resultCode, data);
        	controller.ProcessCodeString(codeString);
        }
        else if (requestCode == SET_USER_ID_CODE)
        {
        	if(resultCode != RESULT_OK)
        	{
        		return;
        	}
        	
        	SharedPreferences sharedPref = PreferenceManager.getDefaultSharedPreferences(this);
    		int id = sharedPref.getInt("UserID", -1544);
    		if(id == -1544)
    		{
    			ToastHelper.ShowToast("Nie udało się ustawić identyfikatora, spróbuj ponownie.", getApplicationContext());
    		}
    		else
    		{
    			    			
    			TextView txtId = (TextView)findViewById(R.id.txtRunnerId);
    			txtId.setText("Twój identyfikator:  " + id);
    		}
        }
        
    }
	
	@Override
	protected void onResume()
	{
		super.onResume();
		// Jeśli powróciliśmy z innej instancji tego activity, która zakończyła bieg
		File file = new File(getFilesDir() + "/CurrentRun.xml");
		if(file.exists() == false)
		{
			ResetRun();
		}
		
		controller.RestoreLastRun();
		
		if(TimeWatcherService.GetIsRunning() == false
				&& controller.GetOrienteeringRun() != null
				&& controller.GetOrienteeringRun().GetIsInProgress() == true)
		{
			startService(new Intent(OrienteeringRunActivity.this,TimeWatcherService.class));
		}
	}
	
	public void StartRun(String runHeader)
	{		
		StartTimer();
		
		TextView txtStatus = (TextView)findViewById(R.id.txtStatus);
		txtStatus.setText("W trakcie biegu. Wystartowano o " +  sdfShort.format(controller.GetOrienteeringRun().GetDateStarted()));
		ToastHelper.ShowToast("Rozpoczęto bieg.", getApplicationContext());
		
		try
		{
			File timeFile = new File(getFilesDir(), "time.log");
			timeFile.delete();
			timeFile.createNewFile();
			if(TimeWatcherService.GetIsRunning() == false)
			{
				startService(new Intent(OrienteeringRunActivity.this,TimeWatcherService.class));
			}
			
		}
		catch(IOException ex)
		{
			Log.e(TAG, "Problem z tworzeniem pliku czasu");
		}
		
	}

	public void Checkpoint()
	{
		TextView txtStatus = (TextView)findViewById(R.id.txtStatus);
		txtStatus.setText("W trakcie biegu. Ostatni punkt: " +  sdfShort.format(controller.GetLastCheckpointTime()));
		ToastHelper.ShowToast("Zarejestrowano punkt kontrolny.", getApplicationContext());
	}

	public void FinishRun()
	{
		TextView txtStatus = (TextView)findViewById(R.id.txtStatus);
		txtStatus.setText("Bieg zakończony " +  sdfLong.format(controller.GetOrienteeringRun().GetDateEnded()));
		ToastHelper.ShowToast("Zakończono bieg.", getApplicationContext());
		
		if(TimeWatcherService.GetIsRunning() == true)
		{
			stopService(new Intent(OrienteeringRunActivity.this,TimeWatcherService.class));
		}		
		if(uiRefreshTimer != null)
		{
			uiRefreshTimer.cancel();
			uiRefreshTimer.purge();
		}				
	}

	public void RestoreRun()
	{
		ToastHelper.ShowToast("Przywrócono ostatni bieg", getApplicationContext());
		
		TextView txtStatus = (TextView)findViewById(R.id.txtStatus);
		TextView txtId = (TextView)findViewById(R.id.txtRunnerId);		
		if(controller.GetOrienteeringRun().GetCheckpoints().size() == 0)
		{
			txtStatus.setText("W trakcie biegu. Wystartowano o " +  sdfShort.format(controller.GetOrienteeringRun().GetDateStarted()));			
		}
		else
		{
			txtStatus.setText("W trakcie biegu. Ostatni punkt: " +  sdfShort.format(controller.GetLastCheckpointTime()));			
		}
		txtId.setText("Twój identyfikator:  " + controller.GetOrienteeringRun().GetRunnerId());
		if(controller.GetOrienteeringRun().GetIsInProgress() == true)
		{
			StartTimer();
		}		
	}
	
	public void DebugStart(View v)
	{
		Random r = new Random();
		controller.ProcessCodeString("ST/Bieg testowy #" +r.nextInt());
				
		
	}
	
	public void DebugCheckpoint(View v)
	{
		Random r = new Random();
		controller.ProcessCodeString("CH/" + r.nextInt(1000));
	}
	
	public void DebugFinish(View v)
	{
		controller.ProcessCodeString("FI/xtra");
	}
	
	public void ExportRun(View v)
	{
		if(controller.GetOrienteeringRun() != null
				&& controller.GetOrienteeringRun().GetIsInProgress() == false)
		{			
			Intent i = new Intent(this, RunExporter.class);
			DatabaseHelper dh = new DatabaseHelper(this);
			i.putExtra("RunId", dh.GetLatestRun().GetId());
			this.startActivity(i);
		}
	}
	
	public void SetUserID(View v)
	{
		if(controller.GetOrienteeringRun() != null &&
        		controller.GetOrienteeringRun().GetIsInProgress() == true)
        		{
        			ToastHelper.ShowToast("Nie możesz zmieniać identyfikatora podczas biegu.", getApplicationContext());
        		}
        		else
        		{
        			startActivityForResult(new Intent(this, UserIdentifierChooser.class), SET_USER_ID_CODE);
        		}
	}
	
	private void StartTimer()
	{
		uiRefreshTimer = new Timer();
		uiRefreshTimer.schedule(
				new TimerTask()
				{
					
					@Override
					public void run()
					{
						runOnUiThread(new Runnable()
						{
							public void run()
							{
								RefreshUI();
							}
						});						
					}
				}, new Date(), 1000);
	}
	
	private void RefreshUI()
	{
		if(controller.GetOrienteeringRun() == null || controller.GetOrienteeringRun().GetDateStarted() == null)
		{
			return;
		}
		
		TextView txtTime = (TextView)findViewById(R.id.txtTime);
		
		Date startTime = controller.GetOrienteeringRun().GetDateStarted();
		Date endTime;
		if(controller.GetOrienteeringRun().GetIsInProgress() == true)
		{
			endTime = new Date();
		}
		else
		{
			endTime = controller.GetOrienteeringRun().GetDateEnded();
		}
		
		long milliseconds = endTime.getTime() - startTime.getTime();
		int seconds = (int) (milliseconds / 1000) % 60 ;
		int minutes = (int) ((milliseconds / (1000*60)) % 60);
		int hours   = (int) ((milliseconds / (1000*60*60)) % 24);
		
		// na wypadek zmiany czasu
		hours = hours < 0 ? 0 : hours;
		minutes = minutes < 0 ? 0 : minutes;
		seconds = seconds < 0 ? 0 : seconds;
		
		String hoursString   = hours > 9 ? hours + "" : "0" + hours;
		String minutesString = minutes > 9 ? minutes + "" : "0" + minutes;
		String secondsString = seconds > 9 ? seconds + "" : "0" + seconds;
		
		txtTime.setText(hoursString + ":" + minutesString + ":" + secondsString);
	
	}
	
	private void ResetRunDialog()
	{
		DialogInterface.OnClickListener dialogClickListener = new DialogInterface.OnClickListener() {			    
		    public void onClick(DialogInterface dialog, int which) {
		        switch (which){
		        case DialogInterface.BUTTON_POSITIVE:
		        	dialog.dismiss();
		        	ResetRun();
		            break;

		        case DialogInterface.BUTTON_NEGATIVE:
		            dialog.dismiss();
		            return;		            
		        }
		    }
		};

		if(controller.GetOrienteeringRun() != null
				&& controller.GetOrienteeringRun().GetIsInProgress() == true)
		{
			AlertDialog.Builder builder = new AlertDialog.Builder(this);
			builder.setTitle("Wyczyścić bieg?").setMessage("Wszystkie dane zarejestrowane w obecnym biegu zostaną skasowane.").setPositiveButton("Tak", dialogClickListener)
			    .setNegativeButton("Nie", dialogClickListener).show();
		}
		else
		{
			ResetRun();
		}
			
		
		
	}
	
	private void ResetRun()
	{
		scanner = new Scanner();
		controller = new OrienteeringRunController(this);
		
		
		TextView txtId = (TextView)findViewById(R.id.txtRunnerId);
		SharedPreferences sharedPref = PreferenceManager.getDefaultSharedPreferences(this);
		int id = sharedPref.getInt("UserID", -1544);
		if(id == -1544)
		{
			txtId.setText("Ustaw identyfikator przed rozpoczęciem biegu");
		}
		else
		{
			txtId.setText("Twój identyfikator:  " + id);
		}
		
		stopService(new Intent(OrienteeringRunActivity.this,TimeWatcherService.class));

		if(uiRefreshTimer != null)
		{
			uiRefreshTimer.cancel();
			uiRefreshTimer.purge();
		}			
		
		TextView txtStatus = (TextView)findViewById(R.id.txtStatus);
		txtStatus.setText("Gotowy do biegu");
		
		TextView txtTime = (TextView)findViewById(R.id.txtTime);
		txtTime.setText("00:00:00");
		
		File file = new File(getFilesDir() + "/CurrentRun.xml");
		if(file.exists() == true)
		{
			file.delete();
		}
	}
	
}
