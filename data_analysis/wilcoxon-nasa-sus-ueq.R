# Load required libraries
library(dplyr)
library(tidyr)
library(rstatix)

# Assume data_long is already loaded with columns: Participant, A, B, and score
# data_long <- read.csv("NASA-TLX.csv", header=TRUE, sep=",", dec = ".")
# data_long <- read.csv("SUS.csv", header=TRUE, sep=",", dec = ".")
data_long <- read.csv("UEQ-S.csv", header=TRUE, sep=",", dec = ".")

# Convert factors
#data_long$Participant <- factor(data_long$participant)
#data_long$Technique <- factor(data_long$technique)
#data_long$Category <- factor(data_long$category)

summary_stats <- data_long %>%
  group_by(technique, category) %>%
  summarise(
    mean_score = mean(score),
    sd_score = sd(score)
  )

print(summary_stats)

# Initialize an empty list to store results
results <- list()

# Perform Wilcoxon signed rank test for each B condition
for (b_level in unique(data_long$category)) {
  # Subset and reshape data for the current B level
  subset_data <- data_long %>% 
    filter(category == b_level) %>%
    pivot_wider(names_from = technique, values_from = score) %>%
    drop_na()  # Remove any rows with NA values
  
  # Check if we have enough paired observations
  if (nrow(subset_data) < 2) {
    warning(paste("Not enough paired observations for condition", b_level))
    next  # Skip to the next iteration of the loop
  }
  
  # Perform Wilcoxon signed rank test
  tryCatch({
    test_result <- wilcox.test(subset_data$AssistVR, subset_data$DiscPIM, paired = TRUE)
    
    # Calculate effect size (r)
    z <- qnorm(test_result$p.value/2)
    r <- abs(z / sqrt(nrow(subset_data)))
    
    # Store results
    results[[b_level]] <- data.frame(
      B = b_level,
      V = test_result$statistic,
      p_value = test_result$p.value,
      r = r,
      n_pairs = nrow(subset_data)
    )
  }, error = function(e) {
    warning(paste("Error in Wilcoxon test for condition", b_level, ":", e$message))
  })
}

# Combine all results into a single data frame
if (length(results) > 0) {
  results_df <- do.call(rbind, results)
  print(results_df)
} else {
  print("No valid results to display.")
}