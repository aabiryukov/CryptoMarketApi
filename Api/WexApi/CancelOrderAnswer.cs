using Newtonsoft.Json.Linq;
using System;

namespace Wex
{
	public class CancelOrderAnswer
	{
		public int OrderId { get; private set; }
		public Funds Funds { get; private set; }

		private CancelOrderAnswer() {}
		public static CancelOrderAnswer ReadFromJObject(JObject jData) {
            if (jData == null) throw new ArgumentNullException(nameof(jData));

			return new CancelOrderAnswer() {
				Funds = Funds.ReadFromJObject(jData["funds"] as JObject),
				OrderId = jData.Value<int>("order_id")
			};
		}
	}
}
