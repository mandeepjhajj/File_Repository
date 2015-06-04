
public class NumberOfStrings {

	public static void main(String[] args) {
		NumberOfStrings s1 = new NumberOfStrings();
		String s= "1322";
		int n=s1.noOfStringsFromIntegerString(s,0,s.length());
		System.out.println(n);
	}

	/*public void countString(String s)
	{
		
		int count[] = new int[s.length()];
		count[s.length()-1]=1;
		count[s.length()-2]=1;
		for(int a: count)
		{
			System.out.println(a);
		}
		for(int i=s.length()-2;i<0;i--)
		{
			count[i]= count[i+1]+ i+2<s.length() ? ((Integer.parseInt(s.substring(i, i+2))<26) ? count[i+2] : 0): 0;
		}
		
		System.out.println("Number of distinct strings "+ count[0]);
	}*/
	public int noOfStringsFromIntegerString(String s,int index,int length)
    {   int count =0;
    	if(index+1>=length)
    		count = 1;
    	else {
    		count = noOfStringsFromIntegerString(s, index+1, length);
    		int temp = Integer.parseInt( s.substring(index,index+2));
    		if(temp < 26){
    			count += noOfStringsFromIntegerString(s, index+2, length);
    		}
    	}
    	return count;
    }
}
