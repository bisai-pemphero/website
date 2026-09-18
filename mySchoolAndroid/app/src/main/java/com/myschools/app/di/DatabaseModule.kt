package com.myschools.app.di

import android.content.Context
import com.myschools.app.data.local.AppDatabase
import com.myschools.app.data.local.LocalAcademicYearDao
import com.myschools.app.data.local.LocalClassDao
import com.myschools.app.data.local.LocalFeesCategoryDao
import com.myschools.app.data.local.LocalPaymentDao
import com.myschools.app.data.local.LocalStudentDao
import com.myschools.app.data.local.LocalTermDao
import dagger.Module
import dagger.Provides
import dagger.hilt.InstallIn
import dagger.hilt.android.qualifiers.ApplicationContext
import dagger.hilt.components.SingletonComponent
import javax.inject.Singleton

@Module
@InstallIn(SingletonComponent::class)
object DatabaseModule {

    @Provides
    @Singleton
    fun provideAppDatabase(@ApplicationContext context: Context): AppDatabase {
        return AppDatabase.getDatabase(context)
    }

    @Provides
    @Singleton
    fun provideStudentDao(database: AppDatabase): LocalStudentDao {
        return database.studentDao()
    }

    @Provides
    @Singleton
    fun provideAcademicYearDao(database: AppDatabase): LocalAcademicYearDao {
        return database.academicYearDao()
    }

    @Provides
    @Singleton
    fun provideTermDao(database: AppDatabase): LocalTermDao {
        return database.termDao()
    }

    @Provides
    @Singleton
    fun provideClassDao(database: AppDatabase): LocalClassDao {
        return database.classDao()
    }

    @Provides
    @Singleton
    fun provideFeesCategoryDao(database: AppDatabase): LocalFeesCategoryDao {
        return database.feesCategoryDao()
    }

    @Provides
    @Singleton
    fun providePaymentDao(database: AppDatabase): LocalPaymentDao {
        return database.paymentDao()
    }
}
