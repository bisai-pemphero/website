package com.myschools.app.data.local

import androidx.room.*
import kotlinx.coroutines.flow.Flow

@Dao
interface LocalStudentDao {
    @Query("SELECT * FROM students WHERE schoolId = :schoolId ORDER BY lastName, firstName")
    fun getAllStudents(schoolId: Int): Flow<List<LocalStudent>>

    @Query("SELECT * FROM students WHERE studentID = :studentId")
    suspend fun getStudentById(studentId: Int): LocalStudent?

    @Query("SELECT * FROM students WHERE schoolId = :schoolId AND admissionNo LIKE :searchQuery OR firstName LIKE :searchQuery OR lastName LIKE :searchQuery")
    fun searchStudents(schoolId: Int, searchQuery: String): Flow<List<LocalStudent>>

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertStudent(student: LocalStudent)

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertAllStudents(students: List<LocalStudent>)

    @Delete
    suspend fun deleteStudent(student: LocalStudent)

    @Query("DELETE FROM students WHERE schoolId = :schoolId")
    suspend fun deleteAllStudents(schoolId: Int)
}

@Dao
interface LocalAcademicYearDao {
    @Query("SELECT * FROM academic_years WHERE schoolId = :schoolId ORDER BY start_year DESC")
    fun getAllAcademicYears(schoolId: Int): Flow<List<LocalAcademicYear>>

    @Query("SELECT * FROM academic_years WHERE academicyearId = :id")
    suspend fun getAcademicYearById(id: Int): LocalAcademicYear?

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertAcademicYear(year: LocalAcademicYear)

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertAllAcademicYears(years: List<LocalAcademicYear>)

    @Delete
    suspend fun deleteAcademicYear(year: LocalAcademicYear)
}

@Dao
interface LocalTermDao {
    @Query("SELECT * FROM terms WHERE schoolId = :schoolId ORDER BY startDate DESC")
    fun getAllTerms(schoolId: Int): Flow<List<LocalTerm>>

    @Query("SELECT * FROM terms WHERE termId = :id")
    suspend fun getTermById(id: Int): LocalTerm?

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertTerm(term: LocalTerm)

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertAllTerms(terms: List<LocalTerm>)

    @Delete
    suspend fun deleteTerm(term: LocalTerm)
}

@Dao
interface LocalClassDao {
    @Query("SELECT * FROM classes WHERE schoolId = :schoolId ORDER BY className")
    fun getAllClasses(schoolId: Int): Flow<List<LocalClass>>

    @Query("SELECT * FROM classes WHERE classID = :classId")
    suspend fun getClassById(classId: Int): LocalClass?

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertClass(cls: LocalClass)

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertAllClasses(classes: List<LocalClass>)

    @Delete
    suspend fun deleteClass(cls: LocalClass)
}

@Dao
interface LocalFeesCategoryDao {
    @Query("SELECT * FROM fees_categories WHERE schoolId = :schoolId ORDER BY category_name")
    fun getAllFeesCategories(schoolId: Int): Flow<List<LocalFeesCategory>>

    @Query("SELECT * FROM fees_categories WHERE categoryId = :id")
    suspend fun getFeesCategoryById(id: Int): LocalFeesCategory?

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertFeesCategory(category: LocalFeesCategory)

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertAllFeesCategories(categories: List<LocalFeesCategory>)

    @Delete
    suspend fun deleteFeesCategory(category: LocalFeesCategory)
}

@Dao
interface LocalPaymentDao {
    @Query("SELECT * FROM payments ORDER BY datePaid DESC")
    fun getAllPayments(): Flow<List<LocalPayment>>

    @Query("SELECT * FROM payments WHERE transanctionId = :id")
    suspend fun getPaymentById(id: Int): LocalPayment?

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertPayment(payment: LocalPayment)

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertAllPayments(payments: List<LocalPayment>)

    @Delete
    suspend fun deletePayment(payment: LocalPayment)
}
