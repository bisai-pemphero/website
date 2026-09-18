package com.myschool.app.data.local

import androidx.datastore.core.DataStore
import androidx.datastore.preferences.core.Preferences
import androidx.datastore.preferences.core.edit
import androidx.datastore.preferences.core.stringPreferencesKey
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.map
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class AuthDataStore @Inject constructor(
    private val dataStore: DataStore<Preferences>
) {
    companion object {
        val ACCESS_TOKEN_KEY = stringPreferencesKey("access_token")
        val USER_ID_KEY = stringPreferencesKey("user_id")
        val ROLE_ID_KEY = stringPreferencesKey("role_id")
        val SCHOOL_ID_KEY = stringPreferencesKey("school_id")
    }

    val accessToken: Flow<String?> = dataStore.data.map { preferences ->
        preferences[ACCESS_TOKEN_KEY]
    }

    val userId: Flow<String?> = dataStore.data.map { preferences ->
        preferences[USER_ID_KEY]
    }

    val roleId: Flow<Int?> = dataStore.data.map { preferences ->
        preferences[ROLE_ID_KEY]?.toIntOrNull()
    }

    val schoolId: Flow<Int?> = dataStore.data.map { preferences ->
        preferences[SCHOOL_ID_KEY]?.toIntOrNull()
    }

    suspend fun saveAuthData(token: String, userId: String, roleId: Int, schoolId: Int) {
        dataStore.edit { preferences ->
            preferences[ACCESS_TOKEN_KEY] = token
            preferences[USER_ID_KEY] = userId
            preferences[ROLE_ID_KEY] = roleId.toString()
            preferences[SCHOOL_ID_KEY] = schoolId.toString()
        }
    }

    suspend fun clearAuthData() {
        dataStore.edit { preferences ->
            preferences.clear()
        }
    }

    suspend fun isLoggedIn(): Boolean {
        return dataStore.data.map { preferences ->
            !preferences[ACCESS_TOKEN_KEY].isNullOrEmpty()
        }.first()
    }
}
