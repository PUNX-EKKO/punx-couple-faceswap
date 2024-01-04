using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.EssentialKit
{
    [SerializeField]
    public class BillingProductPayoutDefinition
    {
        #region Fields

        [SerializeField]
        private     BillingProductPayoutType    m_payoutType;

        [SerializeField]
        private     string                      m_subtype;

        [SerializeField]
        private     double                      m_quantity;

        [SerializeField]
        private     string                      m_data;

        #endregion

        #region Properties

        public BillingProductPayoutType PayoutType => m_payoutType;

        public string Subtype => PropertyHelper.GetValueOrDefault(m_subtype);

        public double Quantity => m_quantity;

        public string Data => PropertyHelper.GetValueOrDefault(m_data);

        #endregion

        #region Constructors

        public BillingProductPayoutDefinition(BillingProductPayoutType payoutType, string subtype = null,
            double quantity = 1, string data = null)
        {
            // Set properties
            m_payoutType    = payoutType;
            m_subtype       = subtype;
            m_quantity      = quantity;
            m_data          = data;
        }

        #endregion

        #region Base methods

        public override string ToString()
        {
            var     sb  = new System.Text.StringBuilder(128);
            sb.Append("BillingProductPayoutDefinition { ")
                .Append($"Type: {PayoutType} ")
                .Append($"Subtype: {Subtype} ")
                .Append($"Quantity: {Quantity}")
                .Append($"Data: {Data}")
                .Append("}");
            return sb.ToString();
        }

        #endregion
    }
}