
public class BinaryTree {

	private class Bnode
	{
		private int value;
		private Bnode left;
		private Bnode right;
		
		Bnode(int value,Bnode left,Bnode right)
		{
			this.value=value;
			this.left=left;
			this.right=right;
		}
		
		public int getValue()
		{
			return this.value;
		}
		
		public Bnode getLeftChild()
		{
			return this.left;
		}
		public Bnode getRightChild()
		{
			return this.right;
		}
		public void setRightChild(Bnode node)
		{
			this.right=node;
		}
		public void setLeftChild(Bnode node)
		{
			this.left=node;
		}
		public boolean AddNode(int newV)
		{
			if(newV==getValue())
				return false;
			else
			{
				if(newV>getValue())
				{
					if(getRightChild()!=null)
						return getRightChild().AddNode(newV);
					else
					{
						setRightChild(new Bnode(newV,null,null));
						return true;
					}
				}
				else
				{
					if(getLeftChild()!=null)
						return getLeftChild().AddNode(newV);
					else
					{
						setLeftChild(new Bnode(newV,null,null)); 
						return true;
					}
				}
			}
			
		}
		public boolean searchNode(int elm)
		{
			if(elm==getValue())
				return true;
			else if(elm>getValue())
		    {
				if(getRightChild()==null)
						return false;
				else
				{
					return getRightChild().searchNode(elm);
				}
			}
			else //if (elm<getValue())
			{
				if(getLeftChild()==null)
					return false;
				else
				{
					return getLeftChild().searchNode(elm);
				}
			}
			//return false;
		}
	}
	
	static private Bnode root;
	
	public boolean createTree(int input[])
	{
		boolean flag=false;
		for(int i=0;i<input.length;i++)
		{
			if(root==null)
				root= new Bnode(input[i],null,null);
			else
			{
				flag= root.AddNode(input[i]);
				
			}
		}	
		return flag;
	}

	public boolean addElement(int newV)
	{
		if(root==null)
		{
			root = new Bnode(newV,null,null);
			return true;
		}
		else
		{
			root.AddNode(newV);
		}
		return true;
	}
	public boolean search(int src)
	{
		if (root==null)
			return false;
		else
		{
			return root.searchNode(src);
		}
		
	}
	public static void main(String[] args) {
		
		BinaryTree obj = new BinaryTree();
		int inputTree[]= {50,2,13,4,1,60,55,76,61,80};
		
		obj.createTree(inputTree);
				
		obj.addElement(10);
		boolean status = obj.search(10);
		obj.printTree(root);

		if(status)
			System.out.println("Element is present");
			
	}
	
	public void printTree(Bnode root)
	{
		
		// In order traversal
		Bnode current = root;
		if(current!=null)
		{
				printTree(current.getLeftChild());
				System.out.println(current.getValue());
				printTree(current.getRightChild());
		}
		
		
	}

}
