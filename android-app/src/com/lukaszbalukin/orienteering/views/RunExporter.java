package com.lukaszbalukin.orienteering.views;

import java.text.SimpleDateFormat;
import java.util.ArrayList;

import com.google.zxing.WriterException;
import com.google.zxing.common.BitMatrix;
import com.google.zxing.qrcode.QRCodeWriter;
import com.google.zxing.qrcode.encoder.ByteMatrix;
import pl.abbd.zpkorienteering.R;
import com.lukaszbalukin.orienteering.models.OrienteeringRun;
import com.lukaszbalukin.orienteering.utils.DataExporter;
import com.lukaszbalukin.orienteering.utils.DatabaseHelper;
import com.lukaszbalukin.orienteering.utils.QRGeneratorTask;

import android.app.Activity;
import android.graphics.Bitmap;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.ImageView;
import android.widget.ProgressBar;
import android.widget.TextView;

public class RunExporter extends Activity
{

	private final int CHECKPOINTS_PER_SEGMENT = 5;
	
	ArrayList<String> runStrings;
	private int totalParts = 1;
	private int currentPart = 1;
	
	@Override
	protected void onCreate(Bundle savedInstanceState)
	{
		// TODO Auto-generated method stub
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_run_exporter);
		
		int idParameter = getIntent().getIntExtra("RunId", 0);
		if(idParameter == 0)
		{
			return;
		}
		DatabaseHelper dh = new DatabaseHelper(this);
        OrienteeringRun run = dh.GetRun(idParameter);        
        if(run == null)
        {
        	return;
        }
        
        // Zapisuje bieg na SD
     	DataExporter de = new DataExporter();
 		de.WriteToSD(run);
 		
 		// Przygotowuje stringi do zakodowania w QR
		runStrings = BuildRunStrings(run);
		
		// Rysuje 1 czesc kodu
		DrawCode();
	}
	
	private ArrayList<String> BuildRunStrings(OrienteeringRun run)
    {
        /* Format
        // nr segmentu | id uczestnika | znacznik oszukiwania (0/1)| idPunktu1-timestamp | idPunktu2-timestamp |(...)
           ##/##|####|#|####-HHmmss|
         * 5 bajtów - nagłówek
         * 80 bajtów - część */
		SimpleDateFormat sdf = new SimpleDateFormat("HHmmss");
		
		String cheatingFlag = run.GetIsCheating() ? "|1|" : "|0|";
        ArrayList<String> segments = new ArrayList<String>();
        
        String header = run.GetRunnerId() + cheatingFlag + sdf.format(run.GetDateStarted()) + "|";

        String segment = header;
        for(int i = 0; i < run.GetCheckpoints().size(); i++)
        {                                           
            segment += run.GetCheckpoints().get(i).GetCheckpointUID();
            segment += "-";
            segment += sdf.format(run.GetCheckpoints().get(i).GetDateVisited());
            segment += ("|");
            
            if (i % (CHECKPOINTS_PER_SEGMENT) == CHECKPOINTS_PER_SEGMENT - 1)
            {
                segments.add(segment);
                segment = "";
            }            
        }
        
        segment += sdf.format(run.GetDateEnded()) + "|";
        segments.add(segment);
        
        

        // Na zakończenie dodaj znaczniki segmentów
        for (int i = 1; i <= segments.size(); i++)
        {
            String seg = segments.get(i - 1);
            seg = i + "/" + segments.size() + "|" + seg;
            segments.set(i - 1, seg);            
        }


        totalParts = segments.size();
        return segments;
    }
	
	public void NextPart(View v)
	{
		if(currentPart + 1 <= totalParts)
		{
			currentPart++;
			DrawCode();
		}
	}
	
	public void PrevPart(View v)
	{
		if(currentPart > 1)
		{
			currentPart--;
			DrawCode();
		}
	}
	
	private void DrawCode()
	{
		TextView label = (TextView)findViewById(R.id.lblCodeSegment);
		label.setText(currentPart + "/" + totalParts);
		
		ImageView img = (ImageView)findViewById(R.id.imgCode);		
		ProgressBar progress = (ProgressBar)findViewById(R.id.progCodeLoading);
		img.setVisibility(4);
		progress.setVisibility(0);
		
		QRGeneratorTask task = new QRGeneratorTask(img, progress, runStrings.get(currentPart - 1));
		task.execute("");
		
		
	}
	
}
