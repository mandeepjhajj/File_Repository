import java.io.*;
import java.util.Scanner;

class Filehandling{
	public void writeFile() 
	{
		try
		{
			FileOutputStream fout = new FileOutputStream("file1.txt");
			String content = "This is fileoutput stream";
			byte s[]= content.getBytes();
			fout.write(s);
			/*int i=10;
			Integer l = (Integer)i;
			byte s= l.byteValue();
			fout.write(s);*/
			fout.close();
		} 
		catch (IOException e) 
		{
			e.printStackTrace();
		}
		System.out.println("File written sucessfully !");
		
	}
	
	public void readFile()
	{
		int i=0;
		try
		{
			System.out.println("Reading from File...");
			FileInputStream fin = new FileInputStream("File1.txt");
			while((i=fin.read())!=-1)
			{
				System.out.print((char)i);
			}
			fin.close();
		}
		catch(IOException e)
		{
			System.out.println(e);
		}
	}
	
}

class bufferedIOStream{
	public void writeBuffer()
	{
		try
		{
			System.out.println("\nWriting data using buffered output stream");
			FileOutputStream fout = new FileOutputStream("file2.txt");
			BufferedOutputStream buff = new BufferedOutputStream(fout);
			String s = "I have Amazon assesment test";
			byte b[]= s.getBytes();
			buff.write(b);
			
			//flush() flushes the data of one stream and send it to another.
			//It is required if we have connected one stream with another.
		
			buff.flush();
			fout.close();
			buff.close();
			System.out.println("File written successfully");
		}
		
		catch(IOException e)
		{
			System.out.println(e);
		}
	}
}

class FileRW{
	public void fileWriter()
	{		
		try
		{
			System.out.println("Writing file using file writer");
			FileWriter w = new FileWriter("file3.txt");
			w.write("Using file writer class to write file");
			w.append(": File 3.txt");
			w.close();
			System.out.println("file writen sucessfully");
		} 
		catch (IOException e) 
		{
			System.out.println(e);
		}
		
	}
	public void fileReader()
	{
		int i =0;
		try
		{
			System.out.println("Reading file3.txt");
			FileReader r = new FileReader("file3.txt");
			while((i=r.read())!=-1)
			{
				System.out.print((char)i);
			}
			r.close();
		}
		catch(IOException e)
		{
			System.out.println(e);
		}
	}
}
public class IOConsole {

	public static void main(String[] args) throws IOException {
	
		int i = System.in.read();
		System.out.println("entered value is : "+(char)i);
		Filehandling f = new Filehandling();
		f.writeFile();
		f.readFile();
		
		bufferedIOStream b= new bufferedIOStream();
		b.writeBuffer();
		
		FileRW r = new FileRW();
		r.fileWriter();
		r.fileReader();
		
		//Scanner class
		
		Scanner scan = new Scanner(System.in);
		System.out.println("Enter name");
		String name = scan.next();
		System.out.println("Enter the roll no");
		int a = scan.nextInt();
		System.out.println("Enter the city");
		String city = scan.next();
		
		System.out.println("Name : "+name+" roll no: "+a+"  City:"+city);
		scan.close();
		
		
		//Scanner sc = new Scanner(System.in).useDelimiter("\\s");
		/*System.out.println("Enter name and roll");
		String nm= scan.next();
		System.out.println(nm); 
		int d= scan.nextInt();
	    System.out.println(d);*/
		
		/*String input = "10 tea 20 coffee 30 tea buiscuits";  
	     Scanner s = new Scanner(input).useDelimiter("\\s");  
	     System.out.println(s.nextInt());  
	     System.out.println(s.next());  
	     System.out.println(s.nextInt());  
	     System.out.println(s.next());  
	     s.close(); */
		//Read the user input from the keyboard
		/*String c=null;
		Console did not worked.......
		try
		{
		
			c = System.console().readLine();
			//System.out.println("Enter your name");
			//String a = c.readLine();
			System.out.println("Entered name is "+c);
		}
		catch(Exception e)
		{
			if(c==null)
			{
				System.out.println("String c is null");
			}
		}*/
	}

}
