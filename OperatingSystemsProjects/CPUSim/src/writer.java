import java.io.BufferedWriter;
import java.io.FileWriter;
import java.io.IOException;
import java.util.Vector;

public class writer {
	String outputName;
	Vector<String> output = new Vector<>();
	int index;
	public writer(String name, int quantemTime){
		outputName = name;
		outputName += quantemTime + ".txt";
		output.addElement("");
		index = 0;
		
	}
	public void append(String s) {
		String line = output.elementAt(index);
		line += s;
		output.removeElementAt(index);
		output.insertElementAt(line, index);

	}

	public void write() {
		BufferedWriter bw = null;
		FileWriter fw = null;

		try {


			fw = new FileWriter(outputName);
			bw = new BufferedWriter(fw);
			for(String line : output){
				bw.write(line);
				bw.newLine();
			}

			System.out.println("Done");

		} catch (IOException e) {

			e.printStackTrace();

		} finally {

			try {

				if (bw != null)
					bw.close();

				if (fw != null)
					fw.close();

			} catch (IOException ex) {

				ex.printStackTrace();

			}

		}		
	}
	public void skipLine() {
		output.addElement("");
		index++;
	}
}
