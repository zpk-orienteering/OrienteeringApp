package com.lukaszbalukin.orienteering.utils;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.LinkedList;

import com.lukaszbalukin.orienteering.models.Checkpoint;
import com.lukaszbalukin.orienteering.models.OrienteeringRun;

import android.content.ContentValues;
import android.content.Context;
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteOpenHelper;
import android.util.Log;

public class DatabaseHelper extends SQLiteOpenHelper
{

	// Wersja
	private static final int DATABASE_VERSION = 15;

	// Nazwa bazy danych
	private static final String DATABASE_NAME = "Orienteering";

	// Tabele
	private static final String TABLE_RUNS = "Runs";
	private static final String TABLE_CHECKPOINTS = "Checkpoints";

	// Kolumny biegów
	private static final String RUN_ID = "id";
	private static final String RUN_RUNNER_ID = "runnerId";
	private static final String RUN_HEADER = "header";
	private static final String RUN_TIMECHANGED= "timeChanged";
	private static final String RUN_DATE_STARTED = "dateStarted";
	private static final String RUN_DATE_ENDED = "dateEnded";

	// Kolumny biegów
	private static final String CHECKPOINT_ID = "id";
	private static final String CHECKPOINT_UID = "uid";
	private static final String CHECKPOINT_ORDER = "visitOrder";
	private static final String CHECKPOINT_RUN_ID = "runId";
	private static final String CHECKPOINT_DATE_VISITED = "dateVisited";

	public DatabaseHelper(Context context)
	{		
		super(context, DATABASE_NAME, null, DATABASE_VERSION);		
	}

	// Creating Tables
	@Override
	public void onCreate(SQLiteDatabase db)
	{
		String createRunsTable = "CREATE TABLE " + TABLE_RUNS + "(" + RUN_ID
				+ " INTEGER PRIMARY KEY AUTOINCREMENT," + RUN_HEADER + " TEXT,"
				+ RUN_RUNNER_ID + " INTEGER, "				
				+ RUN_DATE_STARTED + " TEXT, "
				+ RUN_DATE_ENDED + " TEXT, "
				+ RUN_TIMECHANGED + " INTEGER" 
				+ ")";

		String createCheckpointsTable = "CREATE TABLE " + TABLE_CHECKPOINTS
				+ "(" 
				+ CHECKPOINT_ID + " INTEGER PRIMARY KEY AUTOINCREMENT,"
				+ CHECKPOINT_UID + " INTEGER,"
				+ CHECKPOINT_RUN_ID + " INTEGER,"
				+ CHECKPOINT_ORDER + " INTEGER,"
				+ CHECKPOINT_DATE_VISITED + " TEXT, FOREIGN KEY(" + CHECKPOINT_RUN_ID + ") REFERENCES " + TABLE_RUNS + "(" + RUN_ID + "))";

		db.execSQL(createRunsTable);
		db.execSQL(createCheckpointsTable);
	}

	// Upgrading database
	@Override
	public void onUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
	{
		// Drop older table if existed
		db.execSQL("DROP TABLE IF EXISTS " + TABLE_RUNS);
		db.execSQL("DROP TABLE IF EXISTS " + TABLE_CHECKPOINTS);

		// Create tables again
		onCreate(db);
	}

	/**
	 * @param run
	 *            Wstawiany bieg Wstawia (INSERT) bieg do bazy danych i
	 *            powiązane z nim checkpointy.
	 */
	public void AddRun(OrienteeringRun run)
	{
		SQLiteDatabase db = this.getWritableDatabase();
		SimpleDateFormat dateFormat = new SimpleDateFormat(
				"yyyy-MM-dd HH:mm:ss");
		ContentValues runValues = new ContentValues();
		runValues.put(RUN_HEADER, run.GetRunHeader());
		runValues.put(RUN_RUNNER_ID, run.GetRunnerId());
		runValues.put(RUN_DATE_STARTED, dateFormat.format(run.GetDateStarted()));
		runValues.put(RUN_DATE_ENDED, dateFormat.format(run.GetDateEnded()));
		
		int cheatingFlag = run.GetIsCheating() ? 1 : 0;
		runValues.put(RUN_TIMECHANGED, cheatingFlag);

		long runId = db.insert(TABLE_RUNS, null, runValues);

		for (Checkpoint c : run.GetCheckpoints())
		{
			ContentValues checkpointValues = new ContentValues();
			checkpointValues.put(CHECKPOINT_DATE_VISITED,
					dateFormat.format(c.GetDateVisited()));
			checkpointValues.put(CHECKPOINT_RUN_ID, runId);
			checkpointValues.put(CHECKPOINT_UID, c.GetCheckpointUID());
			checkpointValues.put(CHECKPOINT_ORDER, c.GetOrder());
			db.insert(TABLE_CHECKPOINTS, null, checkpointValues);
		}

		db.close();
	}
	
	public void DeleteRun(OrienteeringRun run)
	{
		this.DeleteRun(run.GetId());
	}
	
	public void DeleteRun(int runId)
	{
		String deleteCheckpoints = 
				"DELETE FROM " + TABLE_CHECKPOINTS
				+ " WHERE " + CHECKPOINT_RUN_ID + " = " + runId;
		
		String deleteRun = 
				"DELETE FROM " + TABLE_RUNS
				+ " WHERE " + RUN_ID + " = " + runId;
		
		SQLiteDatabase db = this.getWritableDatabase();
		db.execSQL(deleteCheckpoints);
		db.execSQL(deleteRun);		
		db.close();
						
				
	}

	public OrienteeringRun GetRun(int id)
	{
		String selectRun = "SELECT "
				+ TABLE_RUNS + "." + RUN_ID + ", "
				+ RUN_HEADER + ", "
				+ RUN_DATE_STARTED + ", "
				+ RUN_DATE_ENDED + ", "
				+ CHECKPOINT_DATE_VISITED + ", "				
				+ CHECKPOINT_ORDER +", "
				+ CHECKPOINT_UID + ", "
				+ RUN_RUNNER_ID +", "
				+ RUN_TIMECHANGED
				+ " FROM " + TABLE_RUNS + " LEFT OUTER JOIN "
				+ TABLE_CHECKPOINTS + " ON " + TABLE_RUNS + "." + RUN_ID
				+ " = " + TABLE_CHECKPOINTS + "." + CHECKPOINT_RUN_ID
				+ " WHERE "
				+ TABLE_RUNS + "." + RUN_ID
				+ " = " + id;
				
				//+ TABLE_CHECKPOINTS + "." + CHECKPOINT_RUN_ID				
				//+ " = " + id
				//+ " OR " 
		SimpleDateFormat dateFormat = new SimpleDateFormat(
				"yyyy-MM-dd HH:mm:ss");

		SQLiteDatabase db = this.getReadableDatabase();
		Cursor cursor = db.rawQuery(selectRun, null);

		ArrayList<Checkpoint> checkpoints = new ArrayList<Checkpoint>();
		OrienteeringRun run = null;
		try
		{
			if (cursor.moveToFirst())
			{
				int runId = cursor.getInt(0);
				int runnerId = cursor.getInt(7);
				Boolean cheating = cursor.getInt(8) == 1 ? true : false;
				String runHeader = cursor.getString(1);
				Date runDateStarted = dateFormat.parse(cursor.getString(2));
				Date runDateEnded = dateFormat.parse(cursor.getString(3));

				do
				{
					if(cursor.isNull(4) == false)
					{
						Date dateVisited = dateFormat.parse(cursor.getString(4));

						Checkpoint c = new Checkpoint(runId, dateVisited, cursor.getInt(5));
						c.SetCheckpointUID(cursor.getInt(6));
						checkpoints.add(c);
					}										
				} while (cursor.moveToNext());

				run = new OrienteeringRun(runDateStarted, runHeader,
						checkpoints);
				run.SetId(runId);
				run.SetDateEnded(runDateEnded);
				run.SetRunnerId(runnerId);
				run.SetIsCheating(cheating);
			}
		} catch (Exception ex)
		{
			Log.e(DATABASE_NAME, "Problem z bazą danych: " + ex.getMessage());
			ex.printStackTrace();
			return null;
		}
		finally
		{
			cursor.close();
			db.close();
		}
		
		return run;
		
	}
	
	public OrienteeringRun GetLatestRun()
	{
		String selectRun = "SELECT "
				+ TABLE_RUNS + "." + RUN_ID + ", "
				+ RUN_HEADER + ", "
				+ RUN_DATE_STARTED + ", "
				+ RUN_DATE_ENDED + ", "
				+ CHECKPOINT_DATE_VISITED + ", "				
				+ CHECKPOINT_ORDER +", "
				+ CHECKPOINT_UID + ", "
				+ RUN_RUNNER_ID +", "
				+ RUN_TIMECHANGED
				+ " FROM " + TABLE_RUNS + " LEFT OUTER JOIN "
				+ TABLE_CHECKPOINTS + " ON " + TABLE_RUNS + "." + RUN_ID
				+ " = " + TABLE_CHECKPOINTS + "." + CHECKPOINT_RUN_ID
				+ " WHERE " + TABLE_RUNS + "." + RUN_ID
				+ " = (SELECT MAX(id) FROM " + TABLE_RUNS + ")";

		SimpleDateFormat dateFormat = new SimpleDateFormat(
				"yyyy-MM-dd HH:mm:ss");

		SQLiteDatabase db = this.getReadableDatabase();
		Cursor cursor = db.rawQuery(selectRun, null);

		ArrayList<Checkpoint> checkpoints = new ArrayList<Checkpoint>();
		OrienteeringRun run = null;
		try
		{
			if (cursor.moveToFirst())
			{
				int runId = cursor.getInt(0);
				int runnerId = cursor.getInt(7);
				Boolean cheating = cursor.getInt(8) == 1 ? true : false;
				String runHeader = cursor.getString(1);
				Date runDateStarted = dateFormat.parse(cursor.getString(2));
				Date runDateEnded = dateFormat.parse(cursor.getString(3));

				do
				{
					if(cursor.isNull(4) == false)
					{
						Date dateVisited = dateFormat.parse(cursor.getString(4));

						Checkpoint c = new Checkpoint(runId, dateVisited, cursor.getInt(5));
						c.SetCheckpointUID(cursor.getInt(6));
						checkpoints.add(c);
					}
				} while (cursor.moveToNext());

				run = new OrienteeringRun(runDateStarted, runHeader,
						checkpoints);
				run.SetId(runId);
				run.SetIsCheating(cheating);
				run.SetDateEnded(runDateEnded);
				run.SetRunnerId(runnerId);
			}
		} catch (Exception ex)
		{
			Log.e(DATABASE_NAME, "Problem z bazą danych: " + ex.getMessage());
			ex.printStackTrace();
			return null;
		}
		finally
		{
			cursor.close();
			db.close();
		}
		
		return run;
	}

	public LinkedList<OrienteeringRun> GetRuns()
	{

		String selectRuns = "SELECT * FROM " + TABLE_RUNS;
		LinkedList<OrienteeringRun> runs = new LinkedList<OrienteeringRun>();

		SQLiteDatabase db = this.getReadableDatabase();
		Cursor cursor = db.rawQuery(selectRuns, null);

		try
		{
			if (cursor.moveToFirst())
			{
				do
				{
					
					String header = cursor.getString(1);
					int runnerId = cursor.getInt(2);
					SimpleDateFormat dateFormat = new SimpleDateFormat(
							"yyyy-MM-dd HH:mm:ss");
					Date dateStarted = dateFormat.parse(cursor.getString(3));
					Date dateEnded = dateFormat.parse(cursor.getString(4));
					OrienteeringRun run = new OrienteeringRun(dateStarted, header,
							new ArrayList<Checkpoint>());
					run.SetId(cursor.getInt(0));
					run.SetDateEnded(dateEnded);
					run.SetRunnerId(runnerId);
					runs.add(run);
					
				} while (cursor.moveToNext());
			}
		} catch (Exception ex)
		{
			Log.e(DATABASE_NAME, "Problem z bazą danych");
		}
		finally
		{
			cursor.close();
			db.close();
		}

		return runs;
	}
}