using System;

namespace SproketEngine {
	static class Program {
		static void Main(string[] args) {
			using(ScrapHeap game = new ScrapHeap()) {
				game.Run();
			}
		}
	}
}
