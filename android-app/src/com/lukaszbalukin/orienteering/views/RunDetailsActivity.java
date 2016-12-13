package com.lukaszbalukin.orienteering.views;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;

import pl.abbd.zpkorienteering.R;
import com.lukaszbalukin.orienteering.models.Checkpoint;
import com.lukaszbalukin.orienteering.models.OrienteeringRun;
import com.lukaszbalukin.orienteering.utils.DatabaseHelper;

import android.os.Bundle;
import android.app.Activity;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.view.View;
import android.widget.TextView;

public class RunDetailsActivity extends Activity {

	OrienteeringRun shownRun;
    @Override
    public void onCreate(Bundle savedInstanceState) 
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_run_details);
        
        DatabaseHelper dh = new DatabaseHelper(this);
        int idParameter = getIntent().getIntExtra("RunId", 0);
        shownRun =  dh.GetRun(idParameter);
        
        if(shownRun == null)
        {
        	return;
        }
        
        SimpleDateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
        
        ((TextView)findViewById(R.id.txtRunEndedDate)).setText(dateFormat.format(shownRun.GetDateEnded()));
        ((TextView)findViewById(R.id.txtRunStartedDate)).setText(dateFormat.format(shownRun.GetDateStarted()));
        ((TextView)findViewById(R.id.txtRunHeader)).setText(shownRun.GetRunHeader());
        ((TextView)findViewById(R.id.txtFullUserID)).setText(shownRun.GetRunnerId() + "");
        
        StringBuilder sb = new StringBuilder();
        dateFormat = new SimpleDateFormat("HH:mm:ss");
        
        ArrayList<Checkpoint> checkpoints = shownRun.GetCheckpoints();
        
        // DEBUG!!!
        //checkpoints.add(new Checkpoint(2, new Date(2013, 10, 18, 14, 0), checkpoints.size()));
        //checkpoints.add(new Checkpoint(2, new Date(2013, 10, 18, 15, 15), checkpoints.size()));
        //shownRun.SetDateEnded(new Date(2013, 10, 18, 15, 40));
        // END DEBUG
        
        for(int i = 0; i < checkpoints.size(); i++)
        {
        	long split = 0;
        	
        	if(i == 0)
        	{
        		split = (checkpoints.get(i).GetDateVisited().getTime() - shownRun.GetDateStarted().getTime());
        	}
        	else
        	{
        		split = (checkpoints.get(i).GetDateVisited().getTime() - checkpoints.get(i - 1).GetDateVisited().getTime());
        	}
        	        
        	AppendSplit(sb, split, "   " + (i+1) + ". ");
        	sb.append("\n");
        }
        
        // ostatni split
        long lastSplit = shownRun.GetDateEnded().getTime() - checkpoints.get(checkpoints.size() - 1).GetDateVisited().getTime();
    	AppendSplit(sb, lastSplit, "\n   Meta: ");
    	sb.append("\n");
    	
    	long totalTime = shownRun.GetDateEnded().getTime() - shownRun.GetDateStarted().getTime();
        sb.append("\n");
        AppendSplit(sb, totalTime, "WYNIK: ");
        
        /*
        sb.append("\n\nKolejność i identyfikatory punktów: \n");
        for(Checkpoint c : shownRun.GetCheckpoints())
        {
        	sb.append("Punkt ");
        	sb.append(c.GetId() + 1);
        	sb.append(": ");
        	sb.append(dateFormat.format((c.GetDateVisited())));
        	sb.append("\n");
        } */
        ((TextView)findViewById(R.id.txtCheckpointTimes)).setText(sb.toString()); 
    }
    
    private void AppendSplit(StringBuilder sb, long split, String header)
	{		
    	split /= 1000;
        long sec  = (split >= 60 ? split % 60 : split);
        long min  = (split = (split / 60)) >= 60 ? split % 60 : split;
        long hour = (split = (split / 60)) >= 24 ? split % 24 : split;
        
    	sb.append(header);
        
    	if(hour > 0)
		{
    		sb.append(hour);
    		sb.append(":");
		}
    	if(min < 10) sb.append("0");
    	sb.append(min);
    	sb.append(":");
    	if(sec < 10) sb.append("0");
    	sb.append(sec);    	    		
	}

	public void GoBack(View v)
    {
    	finish();
    }
    
    public void Delete(View v)
    {
    	DialogInterface.OnClickListener dialogClickListener = new DialogInterface.OnClickListener() {			    
		    public void onClick(DialogInterface dialog, int which) {
		        switch (which){
		        case DialogInterface.BUTTON_POSITIVE:
		        	dialog.dismiss();
		        	DatabaseHelper db = new DatabaseHelper(RunDetailsActivity.this);
		        	db.DeleteRun(shownRun.GetId());
		        	finish();
		            break;

		        case DialogInterface.BUTTON_NEGATIVE:
		            dialog.dismiss();
		            return;		            
		        }
		    }
		};

		AlertDialog.Builder builder = new AlertDialog.Builder(this);
		builder.setTitle("Na pewno skasować?").setMessage("Dane biegu zostaną bezpowrotnie utracone.").setPositiveButton("Tak", dialogClickListener)
		    .setNegativeButton("Nie", dialogClickListener).show();
		

		
		
    	
    }
    
    public void Export(View v)
    {
    	Intent i = new Intent(this, RunExporter.class);		
		i.putExtra("RunId", shownRun.GetId());
		this.startActivity(i);
    }
}
