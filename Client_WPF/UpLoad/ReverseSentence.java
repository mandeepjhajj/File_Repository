
public class ReverseSentence {

	private int indexR=0;
	public String ReverseSen(String given)
	{
		char reverse[]= new char[given.length()];
		char a[]= given.toCharArray();
		int end = a.length-1;
		int firstWord=0;
		
		for(int i=a.length-1;i>=0;i--)
		{
			if(a[i]==' ')
			{
				copyToReverse(reverse,a,i+1,end);
				end=i-1;
				/*System.out.println(end);*/
			}
		}
		while(firstWord<=end)
		{
			reverse[indexR]=a[firstWord];
			firstWord++;
			indexR++;
		}
		return new String(reverse);
	}
	
	public void copyToReverse(char reverse[],char a[], int start, int end)
	{
		while(start<=end)
		{
				reverse[indexR]= a[start];	
				start++;
				indexR++;			
		}	
		reverse[indexR++]=' ';
	}
	public static void main(String[] args) {
		ReverseSentence rs = new ReverseSentence();
		String reverse = rs.ReverseSen("My Name Is Mandeep");
		System.out.println("Reverse of given sentence is  "+ reverse);
	}
}
