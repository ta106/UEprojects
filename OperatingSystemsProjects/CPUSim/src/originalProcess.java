/////////////////////////////////////////
/////////////////////////////////////////
//Authors: Turki , Mohammad
//CS470
//Dr. Hwang
//Process Management // RR algorithm
//Scheduler Class
/////////////////////////////////////////
public class originalProcess extends process {
	
	// Calls its super constructed
	public originalProcess(){
		super();
		
	}
	//Overrides toString to output only that it is PID 0
	@Override public String toString(){
		return("PID 0");
		
	}
	// Overrides decrement so it does not do anything
	@Override public void decrement(){
		
		
	}
	// Overrides waitOn to do nothing
	@Override public void waitOn(int EID){
		
	}

}
