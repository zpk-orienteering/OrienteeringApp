package com.lukaszbalukin.orienteering.controllers;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileWriter;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.NoSuchElementException;
import java.util.StringTokenizer;

import javax.xml.parsers.ParserConfigurationException;

import org.xml.sax.SAXException;

import android.content.SharedPreferences;
import android.os.SystemClock;
import android.preference.PreferenceManager;
import android.util.Log;

import com.lukaszbalukin.orienteering.models.Checkpoint;
import com.lukaszbalukin.orienteering.models.OrienteeringRun;
import com.lukaszbalukin.orienteering.utils.DataExporter;
import com.lukaszbalukin.orienteering.utils.DatabaseHelper;
import com.lukaszbalukin.orienteering.utils.Serializer;
import com.lukaszbalukin.orienteering.utils.ToastHelper;
import com.lukaszbalukin.orienteering.views.OrienteeringRunActivity;

/**
 * @author lukasz
 * Kontroler widoku obecnego biegu.
 * Zapewnia obsługę początku, checkpointów i końca biegu.
 * 
 */
public class OrienteeringRunController extends BaseController
{
	static final String DELIMITERS  		= "/";
	static final String START_TOKEN 		= "ST";
	static final String CHECKPOINT_TOKEN 	= "CH";
	static final String END_TOKEN 			= "FI";
	
	OrienteeringRunActivity activity;
	OrienteeringRun currentRun;	
	
	ArrayList<Checkpoint> visitedCheckpoints = new ArrayList<Checkpoint>();
	
	/**
	 * @param code Wartość zdekodowana z kodu QR.
	 * 
	 * Bierze wartość zdekodowaną z kodu QR i decyduje na podstawie zawartości co z nią zrobić
	 * Na przykład rozpocząć bieg, zarejestrować obecność, zakończyć bieg.
	 */
	public void ProcessCodeString(String code)
	{
		String action;
		String params;
		
		
		try
		{
			StringTokenizer tok = new StringTokenizer(code, DELIMITERS);
			action = tok.nextToken();
			params = tok.nextToken();
			
			if(action.equals(START_TOKEN))
			{
				StartRun(params);				
			}
			else if(action.equals(CHECKPOINT_TOKEN))
			{
				int checkpointId = Integer.parseInt(params);
				CheckPoint(checkpointId);
			}
			else if(action.equals(END_TOKEN))
			{
				FinishRun(params);
			}
		}
		catch(NoSuchElementException ex)
		{
			ToastHelper.ShowToast("Błąd. Zeskanuj kod ponownie.", activity.getApplicationContext());
			Log.e(OrienteeringRunActivity.TAG, "Invalid code");
		}
		
	}
	
	
	/**
	 * @param startInfo Informacje o wyścigu wyświetlane w nagłówku, na przykład jego nazwa
	 * 
	 * Rozpoczyna bieg.
	 */
	private void StartRun(String startInfo)
	{
		if(currentRun != null && currentRun.GetIsInProgress() == true)
		{
			ToastHelper.ShowToast("Już jesteś w trakcie biegu.", activity);
			return;
		}
		
		SharedPreferences sharedPref = PreferenceManager.getDefaultSharedPreferences(activity);
		int id = sharedPref.getInt("UserID", -1544);
		if(id == -1544)
		{
			ToastHelper.ShowToast("Ustaw identyfikator przed rozpoczęciem biegu", activity);
			return;
		}
		
		currentRun = new OrienteeringRun(new Date(), startInfo, new ArrayList<Checkpoint>());
		currentRun.SetIsInProgress(true);
		activity.StartRun(startInfo);
		SaveCurrentRun();
		
		
	}
	
	/**
	 * @param checkpointId Identyfikator punktu kontrolnego zdekodowany z kodu.
	 * 
	 * Obsługuje odwiedznie punktu kontrolnego biegu - Dodaje punkt kontrolny do biegu.
	 */
	private void CheckPoint(int checkpointId)
	{
		if(currentRun == null || currentRun.GetIsInProgress() == false)
		{
			ToastHelper.ShowToast("Musisz być podczas biegu, aby wykonać tą akcję", activity.getApplicationContext());
			Log.e(OrienteeringRunActivity.TAG,
					"Próbwano zarejestrowac punkt kontrolny nie będąc zarejestrowanym w biegu");
			return;
		}		
		
		currentRun.GetCheckpoints().add(new Checkpoint(checkpointId, new Date(), currentRun.GetCheckpoints().size()));
		activity.Checkpoint();
		SaveCurrentRun();
	}
		
	/**
	 * @param finishInfo Informacje z kodu oznaczającego koniec biegu.
	 * 
	 *  Obsługuje koniec biegu - eksportuje bieg do karty SD.
	 */
	private void FinishRun(String finishInfo)
	{
		if(currentRun == null)
		{
			return;
		}
		
		if(currentRun.GetIsInProgress() == false)
		{
			ToastHelper.ShowToast("Bieg już został zakończony.", activity);
			return;
		}
		
		currentRun.SetDateEnded(new Date());
		currentRun.SetIsInProgress(false);
		
		// Weryfikuje zmiany czasów
		VerifyRun();
		
		// Zapisuje bieg na SD
		DataExporter de = new DataExporter();
		de.WriteToSD(currentRun);
		
		// Zapisuje bieg do historii
		DatabaseHelper dh = new DatabaseHelper(activity.getApplicationContext());
		dh.AddRun(this.currentRun);
		
		// Zakańcza bieg od storny interfejsu
		activity.FinishRun();
		
		// Usuwa tymczasowa reprezentacje biegu
		File currentFile = new File(activity.getFilesDir(), "CurrentRun.xml");
		currentFile.delete();
	}

	public OrienteeringRunController(OrienteeringRunActivity activity)
	{
		this.activity = activity;
	}
		
	public OrienteeringRun GetOrienteeringRun()
	{
		return currentRun;
	}
	
	public Date GetLastCheckpointTime()
	{
		if(currentRun.GetCheckpoints().size() == 0)
		{
			return null;
		}
		
		return currentRun.GetCheckpoints().get(currentRun.GetCheckpoints().size() - 1).GetDateVisited();
	}
	
	public List<Checkpoint> GetVisitedCheckpoints()
	{
		return currentRun.GetCheckpoints();
	}


	public void SaveCurrentRun()
	{
		if(currentRun == null)			
		{
			return;
		}
		
		try
		{			
			File file = new File(activity.getFilesDir(), "CurrentRun.xml");

			SharedPreferences sharedPref = PreferenceManager.getDefaultSharedPreferences(activity);
			int runnerId = sharedPref.getInt("UserID", 0);
			currentRun.SetRunnerId(runnerId);
			
			String fileContents = new Serializer().SerializeRun(currentRun);
			
			FileWriter runWriter = new FileWriter(file);
	        BufferedWriter out = new BufferedWriter(runWriter);
	        out.write(fileContents);
	        out.close();
		}
		catch (Exception x)
		{
		}
		
	}
	
	public void RestoreLastRun()
	{
		try
		{
			if(currentRun == null)
			{
				OrienteeringRun lastRun = new Serializer().DeserializeRun(activity.getFilesDir() + "/CurrentRun.xml");
				if(lastRun.GetIsInProgress() == true)
				{
					currentRun = lastRun;
					activity.RestoreRun();					
				}
				
			}
		} catch(Exception x) 
		{
		}
	}
	
	private void VerifyRun()
	{
		File timeFile = new File(activity.getFilesDir(), "time.log");
		long lastTime = 0;
		try
		{			
			 FileInputStream in = new FileInputStream(timeFile);
			 BufferedReader br = new BufferedReader(new InputStreamReader(in));
			 String strLine;
			 
			 while((strLine = br.readLine())!= null)
			 {
				 long thisTime = Long.parseLong(strLine);
				 //Log.d("TIMEWATCHER",strLine);
				 if(thisTime < lastTime)
				 {
					 //Log.d("TIMEWATCHER","User cheating");
					 ToastHelper.ShowToast("Wykryto zmianę zegara systemowego. Bieg może być nieważny.", activity);
					 this.currentRun.SetIsCheating(true);
				 }
				 else
				 {
					 //Log.d("TIMEWATCHER", "OK: " + (thisTime - lastTime));
				 }
				 lastTime = thisTime;
			 }
			 
			 br.close();
			 in.close();
			 
		} catch (IOException e)
		{
		    Log.e("Orienteering", "Problem z weryfikacją czasów");
		}
		
	}
	
}
