using System;

namespace Wex
{
	public enum WexPair
	{
		btc_usd,
		btc_rur,
		btc_eur,

		ltc_btc,
		ltc_usd,
		ltc_rur,
        ltc_eur,

        nmc_btc,
		nvc_btc,
        nvc_usd,
        usd_rur,
		eur_usd,
		ppc_btc,

		dsh_btc,
        dsh_usd,
        dsh_rur,
        dsh_eur,
        dsh_ltc,
        dsh_eth,

        eth_btc,
        eth_usd,
        eth_eur,
        eth_ltc,
        eth_rur,

        bch_usd,
        bch_btc,

        zec_btc,
        zec_usd,

        // Wex coin pairs
        usdet_usd,
        rubet_rub,
        euret_eur,
        btcet_btc,
        ltcet_ltc,
        ethet_eth,
        nmcet_nmc,
        nvcet_nvc,
        ppcet_ppc,
        bchet_bch,

        Unknown
    }

	public static class WexPairHelper
	{
		public static WexPair FromString(string pairName) {
            if (pairName == null) throw new ArgumentNullException(nameof(pairName));
            if (Enum.TryParse(pairName.ToLowerInvariant(), out WexPair ret))
            {
                return ret;
            }

            return WexPair.Unknown;
        }

		public static string ToString(WexPair v) {
			return Enum.GetName(typeof(WexPair), v).ToLowerInvariant();
		}
	}
}
