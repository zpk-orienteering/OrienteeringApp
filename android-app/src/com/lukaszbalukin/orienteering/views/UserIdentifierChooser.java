package com.lukaszbalukin.orienteering.views;

import pl.abbd.zpkorienteering.R;
import com.lukaszbalukin.orienteering.models.OrienteeringRun;
import com.lukaszbalukin.orienteering.utils.ToastHelper;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.preference.PreferenceManager;
import android.view.KeyEvent;
import android.view.View;
import android.view.WindowManager;
import android.view.inputmethod.EditorInfo;
import android.view.inputmethod.InputMethodManager;
import android.widget.EditText;
import android.widget.TextView;

public class UserIdentifierChooser extends Activity
{	
	@Override
	protected void onCreate(Bundle savedInstanceState)
	{
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_user_id_chooser);
		
		EditText idChooser = ((EditText)findViewById(R.id.txtUserID));
		idChooser.setOnEditorActionListener(new EditText.OnEditorActionListener()
		{
			
			public boolean onEditorAction(TextView v, int actionId, KeyEvent event)
			{
				if(actionId == EditorInfo.IME_ACTION_DONE)
				{
					Save(v);
					return true;
				}
				return false;
			}
		});	
	}        
		        
	public void Save(View v)
	{
		TextView userInput = (TextView)findViewById(R.id.txtUserID);
		String inputText = userInput.getText().toString();
		
		try
		{
			int id = Integer.parseInt(inputText);
			SharedPreferences sharedPref = PreferenceManager.getDefaultSharedPreferences(this);
			sharedPref.edit().putInt("UserID", id).commit();
			Intent i = this.getIntent();			
			setResult(RESULT_OK, i);
			finish();
		}
		catch(NumberFormatException ex)
		{
			ToastHelper.ShowToast("Podaj identyfikator w formie liczby", this.getApplicationContext());
		}
		
	}
	
}
