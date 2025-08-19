data_long <- read.csv("NASA-TLX-overall.csv", header=TRUE, sep=",", dec = ".")

# Convert factors
data_long$Participant <- factor(data_long$participant)
data_long$Technique <- factor(data_long$technique)
data_long$Category <- factor(data_long$category)

summary_stats <- data_long %>%
  group_by(Technique) %>%
  summarise(
    mean_score = mean(score),
    sd_score = sd(score)
  )

print(summary_stats)

ggbarplot(data=data_long, x="Technique", y="score", add=c("mean_se", "dotplot"))

# ----------------- I. Test data normality -----------------
# Fit the model
alt_model <- lme(score ~ Technique * Category, random = ~1|Participant, data = data_long)

# Extract residuals
residuals <- resid(alt_model)

# Q-Q plot
qq_plot <- ggplot(data.frame(residuals = residuals), aes(sample = residuals)) +
  stat_qq() +
  stat_qq_line() +
  labs(title = "Q-Q Plot of Residuals")

print(qq_plot)

# Shapiro-Wilk test
shapiro_test <- shapiro.test(residuals)
print(shapiro_test)

# 3. Histogram of residuals (optional, but can be informative)
hist_plot <- ggplot(data.frame(residuals = residuals), aes(x = residuals)) +
  geom_histogram(binwidth = 0.5, fill = "blue", alpha = 0.7) +
  labs(title = "Histogram of Residuals", x = "Residuals", y = "Count")

print(hist_plot)

# ----------------- II. Conduct RM-ANOVA without ART -----------------

# Conduct repeated measures ANOVA
anova_result <- ezANOVA(
  data = data_long,
  dv = .(score),
  wid = .(Participant),
  within = .(Technique, Category),
  detailed = TRUE
)

print(anova_result)

# Post-hoc tests
posthoc_Technique <- pairwise.t.test(data_long$trialcompletiontime, data_long$Technique, paired = TRUE, p.adjust.method = "bonferroni")
print(posthoc_Technique)

posthoc_NumberOfTargets <- pairwise.t.test(data_long$trialcompletiontime, data_long$NumberOfTargets, paired = TRUE, p.adjust.method = "bonferroni")
print(posthoc_NumberOfTargets)

posthoc_Perplexity <- pairwise.t.test(data_long$trialcompletiontime, data_long$Perplexity, paired = TRUE, p.adjust.method = "bonferroni")
print(posthoc_Perplexity)

# ------------------------ III. Conduct ART --------------------------
# Perform ART, add Participant error term for repeated measures ANOVA
# m <- art(trialcompletiontime ~ Technique * NumberOfTargets * Perplexity + Error(Participant), data = data_long)
m <- art(score ~ Technique * Category + (1|Participant), data = data_long)
m

# Run ANOVA on aligned and ranked data
anova_results <- anova(m)
anova_results

# Calculate effect sizes
effect_sizes <- eta_squared(m, partial = TRUE)
effect_sizes

# Combine ANOVA results with effect sizes
combined_results <- cbind(anova_results, "Partial η²" = effect_sizes$Eta2_partial)
print(combined_results)

# Post-hoc tests
# For main effects:
art_main_Technique <- art.con(m, "Technique")
print(summary(art_main_Technique))

art_main_Category <- art.con(m, "Category", adjust = "bonferroni")
print(summary(art_main_Category))

# For interaction effects:
art_interaction_TechniqueCategory <- art.con(m, "Technique:Category", adjust = "bonferroni")
print(summary(art_interaction_TechniqueCategory))