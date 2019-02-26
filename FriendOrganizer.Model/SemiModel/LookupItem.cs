/**
 * This SemiModel Create To provide flexible navigation
 * so we can just set it inside tree/list....
 * then we have DisplayMember to show
 * and Id to refereit when we need to work with the entity which it comes from
 */

namespace FriendOrganizer.Model.SemiModel
{
    public class LookupItem
    {
        public int Id { get; set; }

        public string DisplayMember { get; set; }
    }

    /**
     * NullLookupItem : To support passing null : if we need to do a specific action when we pass null .eg : create new view
     */
    public class NullLookupItem : LookupItem
    {
        public new int? Id => null;
    }
}
