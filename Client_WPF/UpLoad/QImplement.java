import java.util.*;

public class QImplement {

	public static void main(String[] args) {
		
		Queue<String> q = new LinkedList<String>();
		q.add("mandeep");
		q.add("singh");
		q.add("jhajj");
		System.out.println("Elements in queue : ");
		for(String s :q)
		{
			System.out.println(s);
		}
		
		System.out.println("Removing elements from queue ");
		q.remove();
		q.remove();
		
		System.out.println("Remaining elements in queue : ");
		for(String s :q)
		{
			System.out.println(s);
		}
	}

}
