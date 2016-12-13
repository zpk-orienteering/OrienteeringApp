package com.lukaszbalukin.orienteering.views;

import pl.abbd.zpkorienteering.R;
import pl.abbd.zpkorienteering.R.layout;
import pl.abbd.zpkorienteering.R.menu;
import com.lukaszbalukin.orienteering.adapters.RunAdapter;
import com.lukaszbalukin.orienteering.models.OrienteeringRun;

import android.os.Bundle;
import android.app.Activity;
import android.app.ListActivity;
import android.content.Intent;
import android.support.v4.widget.CursorAdapter;
import android.view.Menu;
import android.view.View;
import android.widget.Adapter;
import android.widget.ArrayAdapter;
import android.widget.ListView;

public class RunHistoryActivity extends ListActivity 
{
	
	RunAdapter adapter;

    @Override
	protected void onListItemClick(ListView l, View v, int position, long id)
	{
		// TODO Auto-generated method stub
		super.onListItemClick(l, v, position, id);
		Intent i = new Intent(this, RunDetailsActivity.class);
		OrienteeringRun or =  (OrienteeringRun)l.getItemAtPosition(position);
		i.putExtra("RunId", or.GetId());
		this.startActivity(i);
	}

	@Override
    public void onCreate(Bundle savedInstanceState) 
	{
        super.onCreate(savedInstanceState);
        ListView lv = getListView();
        lv.setCacheColorHint(0);
        lv.setBackgroundResource(R.drawable.bg_aligned_w);
   
        adapter = new RunAdapter(this);        
        setListAdapter(adapter);        
    }

	@Override
	protected void onResume()
	{ 
		super.onResume();		
		adapter = new RunAdapter(this);        
        setListAdapter(adapter);		
	}
		
}
