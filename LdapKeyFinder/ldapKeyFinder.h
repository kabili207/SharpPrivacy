//
// Changelog:
//	- 18.02.2004: Created this file.
//	- 18.02.2004: Added this header for the first beta release.
//
// (C) 2004, Roberto Rossi
// ldapKeyFinder.h

#pragma once

using namespace System;
using namespace System::Runtime::InteropServices;

namespace ldapKeyFinder
{

	//C++ class implementing LDAP connectivity functions
public __gc class KeyFinder 
	{
	public:
		boolean KeyFinder::MyLDAPSearchByID(String* Shostname, Int32 Iport, String* Sattrs, String* Squery);
		String* KeyFinder::MyLDAPSearch(String* Shostname, Int32 Iport, String* Sattrs, String* Squery);
		String* KeyFinder::getKeys()[];
	private:
		String* keyList[];
	};

//Get the result of KeyFinder::MyLDAPSearchByID
String* KeyFinder::getKeys()[]{
	return keyList;
}

//Retrieves the first attribute specified in Sattrs given a query Squery
//
//- returns the value of the attribute searched.
String* KeyFinder::MyLDAPSearch(String* Shostname, Int32 Iport, String* Sattrs, String* Squery)
{
	char* hostname = NULL;
	char* attrs = NULL;
	char* query = NULL;
	int port;
	hostname = (char*)(Marshal::StringToHGlobalAnsi(Shostname)).ToPointer();
	attrs = (char*)(Marshal::StringToHGlobalAnsi(Sattrs)).ToPointer();
	query = (char*)(Marshal::StringToHGlobalAnsi(Squery)).ToPointer();
	port = Iport;
    //-------------------------------------------------------
    //Initialize a sesion. LDAP_PORT is the default port, 389.
    //-------------------------------------------------------
    PCHAR hostName = hostname;
    LDAP* pLdapConnection = NULL;
    
    pLdapConnection = ldap_init(hostName, port);
    
    if (pLdapConnection == NULL)
    {
        //printf("ldap_init failed with 0x%x.\n",LdapGetLastError());
        ldap_unbind(pLdapConnection);
        return NULL;
    }
    else
        ;//printf("ldap_init succeeded \n");
    
    
    //-------------------------------------------------------
    // Set session options.
    //-------------------------------------------------------
    ULONG version = LDAP_VERSION3;
    ULONG numReturns = 100;
	ULONG time = 120;
    ULONG lRtn = 0;
    
    // Set the version to 3.0 (default is 2.0).
    lRtn = ldap_set_option(
                    pLdapConnection,           // Session handle
                    LDAP_OPT_PROTOCOL_VERSION, // Option
                    (void*) &version);         // Option value

    if(lRtn == LDAP_SUCCESS)
        ;//printf("ldap version set to 3.0 \n");
    else
    {
        //printf("SetOption Error:%0lX\n", lRtn);
        ldap_unbind(pLdapConnection);
        return NULL;
    }

    // Set the limit on the number on entries returned to 10.
    lRtn = ldap_set_option(
                    pLdapConnection,       //Session handle
                    LDAP_OPT_SIZELIMIT,    // Option
                    (void*) &numReturns);  // Option value

    if(lRtn == LDAP_SUCCESS)
        ;//printf("Max return entries set to 1 \n");
    else
    {
        ;//printf("SetOption Error:%0lX\n", lRtn);
        ldap_unbind(pLdapConnection);
        return NULL;
    }
    
	
	lRtn = ldap_set_option(
		pLdapConnection,
		LDAP_OPT_TIMELIMIT,
		(void*) &time);
	if(lRtn == LDAP_SUCCESS)
        ;//printf("Max time set to 15 \n");
    else
    {
        //printf("SetOption Error:%0lX\n", lRtn);
        ldap_unbind(pLdapConnection);
        return NULL;
    }
    
    //--------------------------------------------------------
    // Connect to the server.
    //--------------------------------------------------------
    
    lRtn = ldap_connect(pLdapConnection, NULL);
    
    if(lRtn == LDAP_SUCCESS)
        ;//printf("ldap_connect succeeded \n");
    else
    {
        //printf("ldap_connect failed with 0x%lx.\n",lRtn);
        ldap_unbind(pLdapConnection);
        return NULL;
    }
    
    
    //--------------------------------------------------------
    // Bind with credentials
    //--------------------------------------------------------
    PCHAR pMyDN = "";
    /*
	SEC_WINNT_AUTH_IDENTITY secIdent;
 
    secIdent.User = (unsigned char*)pUserName;
    secIdent.UserLength = strlen(pUserName);
    secIdent.Password = (unsigned char*)pPassword;
    secIdent.PasswordLength = strlen(pPassword);
    secIdent.Domain = (unsigned char*)hostName;
    secIdent.DomainLength = strlen(hostName);
    secIdent.Flags = SEC_WINNT_AUTH_IDENTITY_ANSI;
    
    lRtn = ldap_bind_s(
                pLdapConnection,      // Session Handle
                pMyDN,                // Domain DN
                (PCHAR)&secIdent,     // Cedential struct
                LDAP_AUTH_NEGOTIATE); // Auth mode
	
	lRtn = ldap_bind_s(
                pLdapConnection,      // Session Handle
                NULL,                // Domain DN
                NULL,     // Cedential struct
                LDAP_AUTH_NEGOTIATE); // Auth mode
    if(lRtn == LDAP_SUCCESS)
    {
        printf("ldap_bind_s succeeded \n");
        secIdent.Password = NULL; // Remove password ptr.
        pPassword = NULL;         // Remove password ptr.
    }
    else
    {
        printf("ldap_bind_s failed with 0x%lx.\n",lRtn);
        ldap_unbind(pLdapConnection);
        return -1;
    }*/
	 
    //----------------------------------------------------------
    // Perform a synchronous search of fabrikam.com for 
    // all user objects that have a "person" category.
    //----------------------------------------------------------
    ULONG errorCode = LDAP_SUCCESS;
    PCHAR pMyFilter = query;
    LDAPMessage* pSearchResult;

	char* attributes[] = {attrs , NULL};

    errorCode = ldap_search_s(
                    pLdapConnection,    // Session handle
                    pMyDN,              // DN to start search
                    LDAP_SCOPE_SUBTREE, // Scope
                    pMyFilter,          // Filter
					attributes,     // Retreive all attrs
                    0,                  // Get both attrs and values
                    &pSearchResult);    // [out] Search results
    
    if (errorCode != LDAP_SUCCESS)
    {
        //printf("ldap_search_s failed with 0x%0lx \n",errorCode);
        ldap_unbind_s(pLdapConnection);
        if(pSearchResult != NULL)
            ldap_msgfree(pSearchResult);
        return NULL;
    }
    else
        ;//printf("ldap_search succeeded \n");
    
    //----------------------------------------------------------
    // Get the number of entries returned.
    //----------------------------------------------------------
    ULONG numberOfEntries;
    
    numberOfEntries = ldap_count_entries(
                        pLdapConnection,    // Session handle
                        pSearchResult);     // Search result
    
    if(numberOfEntries == NULL)
    {
        //printf("ldap_count_entries failed with 0x%0lx \n",errorCode);
        ldap_unbind_s(pLdapConnection);
        if(pSearchResult != NULL)
            ldap_msgfree(pSearchResult);
        return NULL;
    }
    else
        ;//printf("ldap_count_entries succeeded \n");
    
    //printf("The number of entries is: %d \n", numberOfEntries);
    
    
    //----------------------------------------------------------
    // Loop through the search entries, get, and output all of
    // the attributes.
    //----------------------------------------------------------
    LDAPMessage* pEntry = NULL;
    PCHAR pEntryDN = NULL;
    ULONG iCnt = 0;
    char* sMsg;
    BerElement* pBer = NULL;
    PCHAR pAttribute = NULL;
    PCHAR* ppValue = NULL;
    int firstTwo = 0;
    
    for( iCnt=0; iCnt < numberOfEntries; iCnt++ )
    {
        // Get the first/next entry.
        if( !iCnt )
            pEntry = ldap_first_entry(pLdapConnection, pSearchResult);
        else
            pEntry = ldap_next_entry(pLdapConnection, pEntry);
        
        // Output a status message.
        sMsg = (!iCnt ? "ldap_first_entry" : "ldap_next_entry");
        if( pEntry == NULL )
        {
            //printf("%s failed with 0x%0lx \n", sMsg, LdapGetLastError());
            ldap_unbind_s(pLdapConnection);
            ldap_msgfree(pSearchResult);
            return NULL;
        }
        else
            ;//printf("%s succeeded\n",sMsg);
        
        // Output the entry number.
        //printf("ENTRY NUMBER %i \n", iCnt);
                
        // Get the firt attribute name.
        pAttribute = ldap_first_attribute(
                      pLdapConnection,   // Session handle
                      pEntry,            // Current entry
                      &pBer);            // [out] Current BerElement
        
        // Output all the attribute names for the current object
        // and output values for the first 2 attributes.
        while(pAttribute != NULL)
        {
            // Output the attribute name.
            //printf("     ATTR: %s\n",pAttribute);
            
            // Get the string values.
            if(firstTwo < 1)//numberOfEntries)
            {
                ppValue = ldap_get_values(
                              pLdapConnection,  // Session Handle
                              pEntry,           // Current entry
                              pAttribute);      // Current attribute
            
                //Output the value.
                //printf(": %s", *ppValue);
            }
            
            // Free memory.
            ldap_memfree(pAttribute);
            
            // Get the next attribute name.
            pAttribute = ldap_next_attribute(
                            pLdapConnection,   // Sesion Handle
                            pEntry,            // Current entry
                            pBer);             // Current BerElement
            //printf("\n");
            firstTwo += 1;
        }
        
        if( pBer != NULL )
            ber_free(pBer,0);
        pBer = NULL;
        firstTwo = 0;
    }
    
    
    //----------------------------------------------------------
    // Normal clean up and exit.
    //----------------------------------------------------------
	System::String* keyRetrieved = new System::String(*ppValue);

    ldap_unbind(pLdapConnection);
    ldap_msgfree(pSearchResult);
    ldap_value_free(ppValue);
    return keyRetrieved;
}

//Retrieves from a LDAP server a list of tuples with attributes ("pgpuserid", Sattrs) given a query Squery.
//
//- returns true if no errors occurred.
boolean KeyFinder::MyLDAPSearchByID(String* Shostname, Int32 Iport, String* Sattrs, String* Squery)
{
	char* hostname = NULL;
	char* attrs = NULL;
	char* query = NULL;
	int port;
	hostname = (char*)(Marshal::StringToHGlobalAnsi(Shostname)).ToPointer();
	attrs = (char*)(Marshal::StringToHGlobalAnsi(Sattrs)).ToPointer();
	query = (char*)(Marshal::StringToHGlobalAnsi(Squery)).ToPointer();
	port = Iport;
    //-------------------------------------------------------
    //Initialize a sesion. LDAP_PORT is the default port, 389.
    //-------------------------------------------------------
    PCHAR hostName = hostname;
    LDAP* pLdapConnection = NULL;
    
    pLdapConnection = ldap_init(hostName, port);
    
    if (pLdapConnection == NULL)
    {
        //printf("ldap_init failed with 0x%x.\n",LdapGetLastError());
        ldap_unbind(pLdapConnection);
        return false;
    }
    else
        ;//printf("ldap_init succeeded \n");


    //-------------------------------------------------------
    // Set session options.
    //-------------------------------------------------------
    ULONG version = LDAP_VERSION3;
    ULONG numReturns = 1000;
	ULONG time = 1500;
    ULONG lRtn = 0;
    
    // Set the version to 3.0 (default is 2.0).
    lRtn = ldap_set_option(
                    pLdapConnection,           // Session handle
                    LDAP_OPT_PROTOCOL_VERSION, // Option
                    (void*) &version);         // Option value

    if(lRtn == LDAP_SUCCESS)
        ;//printf("ldap version set to 3.0 \n");
    else
    {
        //printf("SetOption Error:%0lX\n", lRtn);
        ldap_unbind(pLdapConnection);
        return false;
    }

    // Set the limit on the number on entries returned to 10.
    lRtn = ldap_set_option(
                    pLdapConnection,       //Session handle
                    LDAP_OPT_SIZELIMIT,    // Option
                    (void*) &numReturns);  // Option value

    if(lRtn == LDAP_SUCCESS)
        ;//printf("Max return entries set to 1 \n");
    else
    {
        ;//printf("SetOption Error:%0lX\n", lRtn);
        ldap_unbind(pLdapConnection);
        return false;
    }
    
	
	lRtn = ldap_set_option(
		pLdapConnection,
		LDAP_OPT_TIMELIMIT,
		(void*) &time);
	if(lRtn == LDAP_SUCCESS)
        ;//printf("Max time set to 15 \n");
    else
    {
        //printf("SetOption Error:%0lX\n", lRtn);
        ldap_unbind(pLdapConnection);
        return false;
    }

    //--------------------------------------------------------
    // Connect to the server.
    //--------------------------------------------------------
    
    lRtn = ldap_connect(pLdapConnection, NULL);
    
    if(lRtn == LDAP_SUCCESS)
        ;//printf("ldap_connect succeeded \n");
    else
    {
        //printf("ldap_connect failed with 0x%lx.\n",lRtn);
        ldap_unbind(pLdapConnection);
        return false;
    }
    
    
    //--------------------------------------------------------
    // Bind with credentials
    //--------------------------------------------------------
    PCHAR pMyDN = "";
    /*
	SEC_WINNT_AUTH_IDENTITY secIdent;
 
    secIdent.User = (unsigned char*)pUserName;
    secIdent.UserLength = strlen(pUserName);
    secIdent.Password = (unsigned char*)pPassword;
    secIdent.PasswordLength = strlen(pPassword);
    secIdent.Domain = (unsigned char*)hostName;
    secIdent.DomainLength = strlen(hostName);
    secIdent.Flags = SEC_WINNT_AUTH_IDENTITY_ANSI;
    
    lRtn = ldap_bind_s(
                pLdapConnection,      // Session Handle
                pMyDN,                // Domain DN
                (PCHAR)&secIdent,     // Cedential struct
                LDAP_AUTH_NEGOTIATE); // Auth mode
	
	lRtn = ldap_bind_s(
                pLdapConnection,      // Session Handle
                NULL,                // Domain DN
                NULL,     // Cedential struct
                LDAP_AUTH_NEGOTIATE); // Auth mode
    if(lRtn == LDAP_SUCCESS)
    {
        printf("ldap_bind_s succeeded \n");
        secIdent.Password = NULL; // Remove password ptr.
        pPassword = NULL;         // Remove password ptr.
    }
    else
    {
        printf("ldap_bind_s failed with 0x%lx.\n",lRtn);
        ldap_unbind(pLdapConnection);
        return -1;
    }*/
	 
    //----------------------------------------------------------
    // Perform a synchronous search of fabrikam.com for 
    // all user objects that have a "person" category.
    //----------------------------------------------------------
    ULONG errorCode = LDAP_SUCCESS;
    PCHAR pMyFilter = query;
    LDAPMessage* pSearchResult;

	char* attributes[] = {"pgpuserid", attrs, NULL};

    errorCode = ldap_search_s(
                    pLdapConnection,    // Session handle
                    pMyDN,              // DN to start search
                    LDAP_SCOPE_SUBTREE, // Scope
                    pMyFilter,          // Filter
					attributes,              // Retreive all attrs
                    0,                  // Get both attrs and values
                    &pSearchResult);    // [out] Search results
    
    if (errorCode != LDAP_SUCCESS)
    {
        //printf("ldap_search_s failed with 0x%0lx \n",errorCode);
        ldap_unbind_s(pLdapConnection);
        if(pSearchResult != NULL)
            ldap_msgfree(pSearchResult);
        return false;
    }
    else
        ;//printf("ldap_search succeeded \n");
    
    //----------------------------------------------------------
    // Get the number of entries returned.
    //----------------------------------------------------------
    ULONG numberOfEntries;
    
    numberOfEntries = ldap_count_entries(
                        pLdapConnection,    // Session handle
                        pSearchResult);     // Search result
    
    if(numberOfEntries == NULL)
    {
        //printf("ldap_count_entries failed with 0x%0lx \n",errorCode);
        ldap_unbind_s(pLdapConnection);
        if(pSearchResult != NULL)
            ldap_msgfree(pSearchResult);
        return false;
    }
    else
        ;//printf("ldap_count_entries succeeded \n");
    
    //printf("The number of entries is: %d \n", numberOfEntries);
    
    
    //----------------------------------------------------------
    // Loop through the search entries, get, and output all of
    // the attributes.
    //----------------------------------------------------------
    LDAPMessage* pEntry = NULL;
    PCHAR pEntryDN = NULL;
    ULONG iCnt = 0;
    char* sMsg;
    BerElement* pBer = NULL;
    PCHAR pAttribute = NULL;
    PCHAR* ppValue = NULL;
    int firstTwo = 0;
	keyList = new String*[numberOfEntries];
    
    for( iCnt=0; iCnt < numberOfEntries; iCnt++ )
    {
        // Get the first/next entry.
        if( !iCnt )
            pEntry = ldap_first_entry(pLdapConnection, pSearchResult);
        else
            pEntry = ldap_next_entry(pLdapConnection, pEntry);
        
        // Output a status message.
        sMsg = (!iCnt ? "ldap_first_entry" : "ldap_next_entry");
        if( pEntry == NULL )
        {
            //printf("%s failed with 0x%0lx \n", sMsg, LdapGetLastError());
            ldap_unbind_s(pLdapConnection);
            ldap_msgfree(pSearchResult);
            return false;
        }
        else
            ;//printf("%s succeeded\n",sMsg);
        
        // Output the entry number.
        //printf("ENTRY NUMBER %i \n", iCnt);
                
        // Get the firt attribute name.
        pAttribute = ldap_first_attribute(
                      pLdapConnection,   // Session handle
                      pEntry,            // Current entry
                      &pBer);            // [out] Current BerElement
        
        // Output all the attribute names for the current object
        // and output values for the first 2 attributes.
		String* values = new String("");
        while(pAttribute != NULL)
        {
            // Output the attribute name.
            //printf("     ATTR: %s\n",pAttribute);
            
            // Get the string values.
            ppValue = ldap_get_values(
                            pLdapConnection,  // Session Handle
                            pEntry,           // Current entry
                            pAttribute);      // Current attribute
        
            //Output the value.
            //printf(": %s", *ppValue);
            
            
            // Free memory.
            ldap_memfree(pAttribute);
            
            // Get the next attribute name.
            pAttribute = ldap_next_attribute(
                            pLdapConnection,   // Sesion Handle
                            pEntry,            // Current entry
                            pBer);             // Current BerElement
            //printf("\n");
			values = System::String::Concat(values,new System::String(ppValue[0]));
			values = System::String::Concat(values,new System::String("   "));
		}
        
        if( pBer != NULL )
            ber_free(pBer,0);
        pBer = NULL;
        firstTwo = 0;
		keyList[iCnt] = values;
    }
    
    
    //----------------------------------------------------------
    // Normal clean up and exit.
    //----------------------------------------------------------

    ldap_unbind(pLdapConnection);
    ldap_msgfree(pSearchResult);
    ldap_value_free(ppValue);
	return true;
}
}