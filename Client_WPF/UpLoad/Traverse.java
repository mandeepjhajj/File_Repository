import java.util.ArrayList;
import java.util.Comparator;
import java.util.HashMap;
import java.util.Map;
import java.util.TreeMap;

class TreeNode {
	public int value;
	public TreeNode lchild;
	public TreeNode rchild;
	
	public TreeNode(int value) {
		this.value = value;
	}
}

public class Traverse {

	public static void main(String []args) {
		TreeNode root = createTree();
		verticalTraverse(root);
	}
	
	public static TreeNode createTree() {
		TreeNode root = new TreeNode(20);
		root.lchild = new TreeNode(8);
		root.rchild = new TreeNode(22);
		root.lchild.lchild = new TreeNode(5);
		root.lchild.rchild = new TreeNode(3);
		root.lchild.rchild.lchild = new TreeNode(10);
		root.rchild.lchild = new TreeNode(4);
		root.rchild.rchild = new TreeNode(25);
		root.rchild.lchild.rchild = new TreeNode(14);
		return root;
	}
	
	public static void verticalTraverse(TreeNode root) {
		if (null == root)
			return;
		int pos = 0;
		TreeMap<Integer, ArrayList<TreeNode>> map = new TreeMap<Integer, ArrayList<TreeNode>>();
		verticalTraverseUtil(root, map, pos);
		//Map<Integer, ArrayList<TreeNode>> resultMap = sortMapByKey(map);
		StringBuilder sb = new StringBuilder();
		for (ArrayList<TreeNode> list : map.values()) {
			for (TreeNode node : list) {
				sb.append(node.value).append(" ");
			}
			sb.append("\n");
		}
		System.out.println(sb.toString());
	}
	
	public static void verticalTraverseUtil(TreeNode root, TreeMap<Integer, ArrayList<TreeNode>> map, int pos) {
		//in order traverse to ensure the correct output
		ArrayList<TreeNode> col = map.get(pos);
		if (null == col) {
			col = new ArrayList<TreeNode>();
			map.put(pos, col);
		}
		col.add(root);
		if (null != root.lchild) {
			verticalTraverseUtil(root.lchild, map, pos - 1);
		}
		if (null != root.rchild) {
			verticalTraverseUtil(root.rchild, map, pos + 1);
		}
	}
	
	private static Map<Integer, ArrayList<TreeNode>> sortMapByKey(Map<Integer, ArrayList<TreeNode>> map) {
		Map<Integer, ArrayList<TreeNode>> sortMap = new TreeMap<Integer, ArrayList<TreeNode>>(new Comparator<Integer>() {
			public int compare(Integer key1, Integer key2) {  
		        return key1.compareTo(key2);  
		    }
		});
		sortMap.putAll(map);
		return sortMap;
	}
}