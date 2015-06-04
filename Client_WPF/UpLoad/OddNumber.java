import java.util.*;


public class OddNumber {

	public static void main(String[] args) {
	
		int a[] = {1,1,2,2,3,3,3,4,4,5,5,5,5,6,6,6,6,6};
		/*for(int i=0;i<a.length;i++)
		{
			System.out.println(a[i]);
		}*/
		print(a);
		findOdd(a);
		
	}
	public static void findOdd(int a[])
	{
		HashMap<Integer,Integer> ht = new HashMap<Integer,Integer>();
		
		for(int i=0;i<a.length;i++)
		{
			if(!ht.containsKey(a[i]))
				ht.put(a[i], 1);
			else
				ht.put(a[i], ht.get(a[i])+1);
					
		}
		for(int a1 : ht.keySet())
		{
			if(ht.get(a1) % 2 ==1)
				System.out.println("Element with odd number of repetetions :  "+a1);;
		}
	}
	public static void print(int a[])
	{
		HashMap<Integer,Integer> test = new HashMap<Integer,Integer>();
		/*for(int i=0;i<a.length;i++)
		{
			if(!test.containsKey(a[i]))
				test.put(a[i], 1);
			else
				test.put(a[i], test.get(a[i])+1);
					
		}*/
		test.put(1,1);
		test.put(1,2);
		test.put(2,3);
		test.put(3,4);
		for(Map.Entry<Integer,Integer> t : test.entrySet())
		{
			System.out.println(t.getKey() +": "+ t.getValue());
		}
	}

}
