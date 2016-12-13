package com.lukaszbalukin.orienteering.controllers;

import com.lukaszbalukin.orienteering.views.RunHistoryActivity;
import com.lukaszbalukin.orienteering.views.SettingsActivity;

import android.app.Activity;
import android.content.Intent;

/**
 * @author lukasz
 * Bazowa klasa kontrolera.
 */
public class BaseController
{
	/**
	 * @param sourceActivity Activity, które wywołuje tą metodę.
	 * Przechodzi na ekran opcji z dowolnego miejsca aplikacji.
	 */
	public void ShowOptions(Activity sourceActivity)
	{
		Intent i = new Intent(sourceActivity, SettingsActivity.class);
		sourceActivity.startActivity(i);
	}
	
	/**
	 * @param sourceActivity Activity, które wywołuje tą metodę.
	 * Przechodzi na ekran historii z dowolnego miejsca aplikacji.
	 */
	public void ShowHistory(Activity sourceActivity)
	{
		Intent i = new Intent(sourceActivity, RunHistoryActivity.class);
		sourceActivity.startActivity(i);
	}
	
	
}
