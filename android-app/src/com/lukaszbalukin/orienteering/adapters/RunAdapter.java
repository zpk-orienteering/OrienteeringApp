package com.lukaszbalukin.orienteering.adapters;

import java.util.LinkedList;
import java.util.List;

import com.lukaszbalukin.orienteering.models.OrienteeringRun;
import com.lukaszbalukin.orienteering.utils.DatabaseHelper;

import android.R;
import android.app.Activity;
import android.content.Context;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.TextView;

public class RunAdapter extends BaseAdapter
{
	Activity ctx;
	LinkedList<OrienteeringRun> runs;
	public RunAdapter(Activity ctx)
	{
		super();
		this.ctx = ctx;
		DatabaseHelper dh = new DatabaseHelper(ctx);
		runs = dh.GetRuns();
	}

	public int getCount()
	{
		return runs.size();
	}

	public Object getItem(int arg0)
	{		// 
		return runs.get(arg0);
	}

	public long getItemId(int arg0)
	{
		return arg0;
	}

	public View getView(int position, View convertView, ViewGroup parent)
	{
		
		View view = convertView; 
		if (view == null) 
		     view = ctx.getLayoutInflater().inflate(android.R.layout.simple_list_item_1, null);
		((TextView)view.findViewById(android.R.id.text1)).setText(runs.get(position).GetRunHeader());
		return view;		       
	}

}
