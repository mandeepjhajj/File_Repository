import java.net.*;
import java.util.*;

public class AnagramSolver {

	
	public static void main(String[] args) throws Exception {

		URL url = new URL("http://www.andrew.cmu.edu/course/15-121/dictionary.txt");
		
		Scanner sc = new Scanner(url.openStream());
		TreeMap<String,ArrayList<String>> tmap = new TreeMap<String,ArrayList<String>>();
		ArrayList<String> anagram=null;
		while(sc.hasNext())
		{
			String word = sc.next();
			String sortW = sortString(word);
			
			if(!tmap.containsKey(sortW))
			{
			    anagram = new ArrayList<String>();
				anagram.add(word);
				tmap.put(sortW, anagram);
			}	
			else
			{
				anagram = tmap.get(sortW);
				anagram.add(word);
				tmap.put(sortW, anagram);
			}
		}
		
		System.out.println(tmap.get(sortString("bears"))); 
	}
	public static String sortString(String a)
	{
		char[] sort = a.toCharArray();
		Arrays.sort(sort);
		return new String(sort);
	}

}
