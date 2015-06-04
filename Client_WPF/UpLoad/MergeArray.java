
public class MergeArray {

	private int[] temp= new int[20];
	private int p1=0,p2=0,f=0;
	public int[] mergeSort(int a[], int b[])
	{
		if(p1>=a.length && p2<b.length)
		{
			temp[f]=b[p2];
			p2++;
			f++;
			return mergeSort(a,b);
		}
		if(p2>=b.length && p1<a.length)
		{
			temp[f]=a[p1];
			p1++;
			f++;
			return mergeSort(a,b);
		}
		
		if(p1 >=a.length && p2>=b.length)
		{
			return temp;
		}
		if(a[p1]>b[p2])
		{
			temp[f]=b[p2];
			p2++;
			f++;
			return mergeSort(a,b);
		}
		else
		{
			temp[f]=a[p1];
			p1++;
			f++;
			return mergeSort(a,b);
		}
	}
	public static void main(String[] args) {
		int a[]= new int[10];
		int b[] = new int [10];
		int j=0;
		for(int i=0;i<70;i=i+7)
		{
			a[j]=i;
			j++;
		}
		int k=0;
		for(int i=0;i<30;i=i+3)
		{
			b[k]=i;
			k++;
		}
		MergeArray obj = new MergeArray();
		int res[]=obj.mergeSort(a,b);
		for(int i=0;i<res.length;i++)
		{
			System.out.println(res[i]);
		}
	}

}
