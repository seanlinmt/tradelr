using System;

namespace tradelr.Models.users
{
    [Flags]
    public enum UserPermission
    {
        NONE                        = 0,
        NETWORK_SETTINGS            = 0x00000004, // modify account settings // done
        NETWORK_STORE               = 0x00040000, // modify store settings // done

        INVENTORY_ADD               = 0x00000008, // add products // done
        INVENTORY_MODIFY            = 0x00000010, // edit products // done
        INVENTORY_VIEW              = 0x00000020, // view inventory // done

        INVOICES_ADD                = 0x00000080, // add invoices // done
        TRANSACTION_MODIFY          = 0x00000100, // edit transactions // done
        TRANSACTION_SEND            = 0x00000200, // send transactions  // done
        TRANSACTION_VIEW            = 0x00000400, // view transactions

        ORDERS_ADD                  = 0x00000800, // add orders // done

        CONTACTS_ADD                = 0x00008000, // add domain contacts // done
        CONTACTS_MODIFY             = 0x00010000, // modify domain contacts // done
        CONTACTS_VIEW               = 0x00020000, // view domain contacts // done

        //PAYMENT_VIEW                = 0x00080000, // view payment history // done

        // defaults
        ADMIN = INVENTORY_ADD | INVENTORY_MODIFY | INVENTORY_VIEW | NETWORK_SETTINGS | NETWORK_STORE |
                INVOICES_ADD | TRANSACTION_MODIFY | TRANSACTION_SEND | TRANSACTION_VIEW |
                ORDERS_ADD | CONTACTS_ADD | CONTACTS_MODIFY | CONTACTS_VIEW ,
        USER = NONE
    }

    public static class UserPermissionHelper
    {
        public static bool HasPermission(this UserPermission perm, UserPermission permvalue)
        {
            if((perm & permvalue) == permvalue)
            {
                return true;
            }
            return false;
        }

    }
}