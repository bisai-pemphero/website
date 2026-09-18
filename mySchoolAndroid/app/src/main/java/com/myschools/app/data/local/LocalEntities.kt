package com.myschools.app.data.local

import androidx.room.Entity
import androidx.room.PrimaryKey

@Entity(tableName = "students")
data class LocalStudent(
    @PrimaryKey val studentID: Int,
    val admissionNo: String,
    val firstName: String,
    val middleName: String?,
    val lastName: String,
    val gender: String?,
    val dateOfBirth: String?,
    val currentClassID: Int?,
    val className: String?, // Denormalized for easier display
    val schoolId: Int,
    val status: String?
)

@Entity(tableName = "academic_years")
data class LocalAcademicYear(
    @PrimaryKey val academicyearId: Int,
    val start_year: String,
    val end_year: String,
    val schoolId: Int
)

@Entity(tableName = "terms")
data class LocalTerm(
    @PrimaryKey val termId: Int,
    val termName: String,
    val academicYearId: Int,
    val startDate: String,
    val endDate: String,
    val isActive: String,
    val schoolId: Int
)

@Entity(tableName = "classes")
data class LocalClass(
    @PrimaryKey val classID: Int,
    val className: String,
    val level: String?,
    val section: String?,
    val schoolId: Int
)

@Entity(tableName = "fees_categories")
data class LocalFeesCategory(
    @PrimaryKey val categoryId: Int,
    val category_name: String,
    val amount: Double,
    val academicLevel: String,
    val className: String?,
    val schoolId: Int
)

@Entity(tableName = "payments")
data class LocalPayment(
    @PrimaryKey val transanctionId: Int,
    val feesId: Int,
    val paid: Double,
    val balance: Double,
    val paymentMode: String?,
    val paidBy: String?,
    val receipNumber: String?,
    val datePaid: String,
    val postedBy: String?,
    val studentName: String?, // Denormalized
    val admissionNo: String? // Denormalized
)
