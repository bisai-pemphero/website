package com.myschools.app.data.local

import android.content.Context
import androidx.room.Database
import androidx.room.Room
import androidx.room.RoomDatabase

@Database(
    entities = [
        LocalStudent::class,
        LocalAcademicYear::class,
        LocalTerm::class,
        LocalClass::class,
        LocalFeesCategory::class,
        LocalPayment::class
    ],
    version = 1,
    exportSchema = false
)
abstract class AppDatabase : RoomDatabase() {
    abstract fun studentDao(): LocalStudentDao
    abstract fun academicYearDao(): LocalAcademicYearDao
    abstract fun termDao(): LocalTermDao
    abstract fun classDao(): LocalClassDao
    abstract fun feesCategoryDao(): LocalFeesCategoryDao
    abstract fun paymentDao(): LocalPaymentDao

    companion object {
        @Volatile
        private var INSTANCE: AppDatabase? = null

        fun getDatabase(context: Context): AppDatabase {
            return INSTANCE ?: synchronized(this) {
                val instance = Room.databaseBuilder(
                    context.applicationContext,
                    AppDatabase::class.java,
                    "myschools_database"
                )
                    .fallbackToDestructiveMigration()
                    .build()
                INSTANCE = instance
                instance
            }
        }
    }
}
